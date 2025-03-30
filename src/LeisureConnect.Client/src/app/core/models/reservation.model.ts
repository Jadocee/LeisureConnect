import { Guest } from "./guest.model";
import { ReservationItem } from "./reservation_item.model";

export interface ReservationRequest {
  customerFirstName: string;
  customerLastName: string;
  customerAddress: string;
  customerPhoneNumber: string;
  customerEmail?: string;
  customerCityId?: number;
  hotelId: number;
  reservationType: string;
  paymentMethodId?: number;
  paymentReference?: string;
  currencyId: number;
  reservationItems: ReservationItem[];
  guests: Guest[];
}