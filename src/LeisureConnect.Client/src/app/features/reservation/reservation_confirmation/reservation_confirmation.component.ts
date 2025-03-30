import { Component, OnInit, DestroyRef, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, NavigationExtras } from '@angular/router';
import { ReservationService } from '../../../core/services/reservation.service';
import { finalize, takeUntil } from 'rxjs/operators';
import { ReservationResponse } from '../../../core/models/reservation_response.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-reservation-confirmation',
  templateUrl: './reservation_confirmation.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class ReservationConfirmationComponent implements OnInit {
  reservation = signal<ReservationResponse | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private reservationService = inject(ReservationService);
  private destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.initializeReservation();
  }

  private initializeReservation(): void {
    const navigation = this.router.getCurrentNavigation();
    const reservationData = navigation?.extras?.state?.['reservationData'] as ReservationResponse | undefined;
    
    if (reservationData) {
      this.reservation.set(reservationData);
      return;
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadReservation(+id);
    } else {
      this.errorMessage.set('No reservation ID provided');
    }
  }

  private loadReservation(id: number): void {
    this.isLoading.set(true);
    
    this.reservationService.getReservation(id)
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.isLoading.set(false))
      )
      .subscribe({
        next: (reservation) => {
          this.reservation.set(reservation);
        },
        error: (error: HttpErrorResponse) => {
          console.error('Error loading reservation', error);
          this.errorMessage.set(
            `Failed to load reservation details: ${error.status === 404 ? 
              'Reservation not found' : 
              'An error occurred. Please try again.'}`
          );
        }
      });
  }

  printConfirmation(): void {
    window.print();
  }

  createNewReservation(): void {
    this.router.navigate(['/reservation/create']);
  }
  
  navigateToHome(): void {
    this.router.navigate(['/']);
  }
}