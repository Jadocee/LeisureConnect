import { Component, OnInit, computed, effect, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

import { Hotel } from '../../../core/models/hotel.model';
import { AdvertisedPackage } from '../../../core/models/advertised_package.model';
import { PaymentMethod } from '../../../core/models/payment_method.model';
import { HotelService } from '../../../core/services/hotel.service';
import { AdvertisedPackageService } from '../../../core/services/advertised_package.service';
import { ReservationService } from '../../../core/services/reservation.service';
import { PaymentMethodService } from '../../../core/services/payment_method.service';
import { ReservationRequest } from '../../../core/models/reservation.model';

// Interfaces
interface LoadingState {
  hotels: boolean;
  packages: boolean;
  paymentMethods: boolean;
  submission: boolean;
}

interface SearchFilters {
  searchText: string;
  selectedType: string;
}

@Component({
  selector: 'app-reservation-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './reservation_form.component.html',
})
export class ReservationFormComponent implements OnInit {
  // Inject services
  private fb = inject(FormBuilder);
  private hotelService = inject(HotelService);
  private packageService = inject(AdvertisedPackageService);
  private reservationService = inject(ReservationService);
  private paymentMethodService = inject(PaymentMethodService);
  private router = inject(Router);

  // Form group
  form!: FormGroup;
  
  // State signals
  hotels = signal<Hotel[]>([]);
  availablePackages = signal<AdvertisedPackage[]>([]);
  paymentMethods = signal<PaymentMethod[]>([]);
  
  loadingState = signal<LoadingState>({
    hotels: false,
    packages: false,
    paymentMethods: false,
    submission: false
  });
  
  isSubmitted = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  minStartDate = new Date();
  minEndDate = new Date(new Date().setDate(new Date().getDate() + 1));
  
  searchFilters = signal<SearchFilters>({
    searchText: '',
    selectedType: 'all'
  });
  
  packageTypes = [
    { id: 'all', name: 'All Types' },
    { id: 'standard', name: 'Standard Packages' },
    { id: 'custom', name: 'Custom Packages' }
  ];

  // Computed values
  filteredPackages = computed(() => {
    const packages = this.availablePackages();
    const { searchText, selectedType } = this.searchFilters();

    return packages.filter(pkg => {
      // Filter by search text
      const matchesSearch = !searchText.trim() ||
        pkg.name.toLowerCase().includes(searchText.toLowerCase()) ||
        pkg.description.toLowerCase().includes(searchText.toLowerCase());

      // Filter by package type
      const matchesType = selectedType === 'all' ||
        (selectedType === 'standard' && pkg.isStandardPackage) ||
        (selectedType === 'custom' && !pkg.isStandardPackage);

      return matchesSearch && matchesType;
    });
  });

  stayLength = computed(() => {
    const startDate = this.form.get('dateRange.startDate')?.value;
    const endDate = this.form.get('dateRange.endDate')?.value;
    
    if (!startDate || !endDate) return 0;

    const diffTime = Math.abs(endDate.getTime() - startDate.getTime());
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  });

  isLoading = computed(() => {
    const state = this.loadingState();
    return state.hotels || state.packages || state.paymentMethods || state.submission;
  });

  // Private flags to prevent multiple loads
  private _hotelsLoaded = false;
  private _paymentMethodsLoaded = false;
  
  constructor() {
    this.initForm();
    
    // Effect to handle hotel ID change
    effect(() => {
      const hotelId = this.form.get('reservationDetails.hotelId')?.value;
      if (hotelId) {
        this.loadAvailablePackages(hotelId);
      }
    });
    
    // Effect to update end date when start date changes
    effect(() => {
      const startDate = this.form.get('dateRange.startDate')?.value;
      const endDate = this.form.get('dateRange.endDate')?.value;
      
      if (startDate && endDate && startDate >= endDate) {
        const newEndDate = new Date(startDate);
        newEndDate.setDate(newEndDate.getDate() + 1);
        this.form.get('dateRange.endDate')?.setValue(newEndDate);
      }
    });
  }

  ngOnInit(): void {
    this.loadHotels();
    this.loadPaymentMethods();
  }

  private initForm(): void {
    this.form = this.fb.group({
      customerInfo: this.fb.group({
        customerFirstName: ['', Validators.required],
        customerLastName: ['', Validators.required],
        customerAddress: ['', Validators.required],
        customerPhoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9]{10,15}$/)]],
        customerEmail: ['', [Validators.email]]
      }),
      
      reservationDetails: this.fb.group({
        hotelId: [null, Validators.required],
        reservationType: ['Phone'],
        paymentMethodId: [null],
        paymentReference: [''],
        currencyId: [1]
      }),
      
      dateRange: this.fb.group({
        startDate: [this.minStartDate, Validators.required],
        endDate: [this.minEndDate, Validators.required]
      }),
      
      reservationItems: this.fb.array([]),
      
      guests: this.fb.array([
        this.createGuestFormGroup(true)
      ])
    });
  }

  // Form array getters
  get reservationItemsArray(): FormArray {
    return this.form.get('reservationItems') as FormArray;
  }

  get guestsArray(): FormArray {
    return this.form.get('guests') as FormArray;
  }

  // Form group creators
  private createGuestFormGroup(isPrimary: boolean = false): FormGroup {
    return this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.email],
      phoneNumber: ['', Validators.pattern(/^\+?[0-9]{10,15}$/)],
      address: [''],
      isPrimaryGuest: [isPrimary]
    });
  }

  private createReservationItemGroup(packageId: number): FormGroup {
    return this.fb.group({
      packageId: [packageId],
      serviceItemId: [null],
      quantity: [1, [Validators.required, Validators.min(1)]],
      startDate: [this.form.get('dateRange.startDate')?.value, Validators.required],
      endDate: [this.form.get('dateRange.endDate')?.value, Validators.required],
      specialRequests: ['']
    });
  }

  // UI update methods
  updateSearchFilters(field: keyof SearchFilters, value: string): void {
    this.searchFilters.update(filters => ({ ...filters, [field]: value }));
  }
  
  // Event handlers for template
  onSearchTextChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.updateSearchFilters('searchText', input.value);
  }
  
  onPackageTypeChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.updateSearchFilters('selectedType', select.value);
  }

  // Action methods
  addPackage(packageId: number): void {
    const selectedPackage = this.availablePackages().find(p => p.packageId === packageId);
    if (!selectedPackage) return;

    this.reservationItemsArray.push(this.createReservationItemGroup(packageId));
  }

  removeReservationItem(index: number): void {
    this.reservationItemsArray.removeAt(index);
  }

  addGuest(): void {
    this.guestsArray.push(this.createGuestFormGroup());
  }

  removeGuest(index: number): void {
    // Don't remove if it's the only guest
    if (this.guestsArray.length <= 1) return;

    this.guestsArray.removeAt(index);

    // Ensure there's always a primary guest
    const primaryExists = this.guestsArray.controls.some(
      control => control.get('isPrimaryGuest')?.value
    );

    if (!primaryExists && this.guestsArray.length > 0) {
      this.guestsArray.at(0).get('isPrimaryGuest')?.setValue(true);
    }
  }

  setPrimaryGuest(index: number): void {
    this.guestsArray.controls.forEach((control, i) => {
      control.get('isPrimaryGuest')?.setValue(i === index);
    });
  }

  // Helper methods
  getTotalPrice(pricePerNight: number): number {
    return pricePerNight * this.stayLength();
  }

  findPackage(packageId: number): AdvertisedPackage | null {
    return this.availablePackages().find(pkg => pkg.packageId === packageId) || null;
  }

  // Service methods
  private loadAvailablePackages(hotelId: number): void {
    const startDate = this.form.get('dateRange.startDate')?.value;
    const endDate = this.form.get('dateRange.endDate')?.value;
    if (!startDate || !endDate) return;

    this.loadingState.update(state => ({ ...state, packages: true }));
    
    this.packageService
      .getAvailableAdvertisedPackages(hotelId, startDate, endDate)
      .pipe(finalize(() => {
        this.loadingState.update(state => ({ ...state, packages: false }));
      }))
      .subscribe({
        next: (packages) => {
          this.availablePackages.set(packages);
        },
        error: (error) => {
          console.error("Error loading packages:", error);
          this.errorMessage.set("Unable to load available packages. Please try again or contact support.");
        },
      });
  }

  private loadPaymentMethods(): void {
    if (this._paymentMethodsLoaded) return;
    this._paymentMethodsLoaded = true;
    
    this.loadingState.update(state => ({ ...state, paymentMethods: true }));
    
    this.paymentMethodService
      .getAllPaymentMethods()
      .pipe(finalize(() => {
        this.loadingState.update(state => ({ ...state, paymentMethods: false }));
      }))
      .subscribe({
        next: (methods) => {
          this.paymentMethods.set(methods);
        },
        error: (error) => {
          console.error("Error loading payment methods:", error);
          this.errorMessage.set("Unable to load payment methods. Please try again or contact support.");
        },
      });
  }

  private loadHotels(): void {
    if (this._hotelsLoaded) return;
    this._hotelsLoaded = true;
    
    this.loadingState.update(state => ({ ...state, hotels: true }));
    
    this.hotelService
      .getAllHotels()
      .pipe(finalize(() => {
        this.loadingState.update(state => ({ ...state, hotels: false }));
      }))
      .subscribe({
        next: (hotels) => {
          this.hotels.set(hotels);
        },
        error: (error) => {
          console.error("Error loading hotels:", error);
          this.errorMessage.set("Unable to load hotels. Please try again or contact support.");
        },
      });
  }

  onSubmit(): void {
    this.isSubmitted.set(true);
    this.errorMessage.set('');
    
    if (!this.form.valid) {
      this.form.markAllAsTouched();
      this.scrollToFirstError();
      return;
    }
    
    if (this.reservationItemsArray.length === 0) {
      this.errorMessage.set('Please add at least one package or service to the reservation.');
      return;
    }

    const formValue = this.form.value;
    const hotelId = formValue.reservationDetails?.hotelId;
    
    if (!hotelId) {
      this.errorMessage.set('Hotel selection is required.');
      return;
    }

    // Prepare the reservation request
    const reservationRequest: ReservationRequest = {
      ...formValue.customerInfo,
      hotelId,
      reservationType: formValue.reservationDetails?.reservationType || 'Phone',
      paymentMethodId: formValue.reservationDetails?.paymentMethodId || undefined,
      paymentReference: formValue.reservationDetails?.paymentReference || '',
      currencyId: formValue.reservationDetails?.currencyId || 1,
      
      reservationItems: formValue.reservationItems?.map((item: any) => ({
        packageId: item.packageId || undefined,
        serviceItemId: item.serviceItemId || undefined,
        quantity: item.quantity,
        startDate: item.startDate.toISOString(),
        endDate: item.endDate.toISOString(),
        specialRequests: item.specialRequests || ''
      })) || [],
      
      guests: formValue.guests?.map((guest: any) => ({
        firstName: guest.firstName,
        lastName: guest.lastName,
        email: guest.email || '',
        phoneNumber: guest.phoneNumber || '',
        address: guest.address || '',
        isPrimaryGuest: guest.isPrimaryGuest
      })) || [],
    };

    this.loadingState.update(state => ({ ...state, submission: true }));
    
    this.reservationService.createReservation(reservationRequest)
      .pipe(
        finalize(() => {
          this.loadingState.update(state => ({ ...state, submission: false }));
        }),
        catchError((error: HttpErrorResponse) => {
          console.error('Error creating reservation', error);
          this.errorMessage.set(error.error?.detail || 'Failed to create reservation. Please try again.');
          return of(null);
        })
      )
      .subscribe(response => {
        if (response) {
          this.router.navigate(['/reservation/confirmation', response.reservationId], {
            state: { reservationData: response }
          });
        }
      });
  }

  private scrollToFirstError(): void {
    setTimeout(() => {
      const firstInvalidControl = document.querySelector('.ng-invalid.ng-touched');
      if (firstInvalidControl) {
        (firstInvalidControl as HTMLElement).scrollIntoView({
          behavior: 'smooth',
          block: 'center'
        });
      }
    });
  }

  // Utility methods
  formatDate(date: any): string {
    if (!date) return '';
    
    if (date instanceof Date) {
      return date.toISOString().split('T')[0]; // Format as YYYY-MM-DD
    }

    if (typeof date === 'string') {
      try {
        return new Date(date).toISOString().split('T')[0];
      } catch (e) {
        console.error('Invalid date string:', date, e);
        return date; // Return as is if invalid
      }
    }

    return ''; // Fallback for any other type
  }
}