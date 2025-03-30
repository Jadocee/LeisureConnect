import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ReservationFormComponent } from './features/reservation/reservation_form/reservation_form.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ReservationFormComponent],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'LeisureConnect.Client';
}
