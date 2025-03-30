export interface ReservationResponse {
    reservationId: number;
    reservationNumber: string;
    reservationDate: string;
    totalAmount: number;
    depositAmount: number;
    customerFullName: string;
    hotelName: string;
    status: string;
    currencyCode: string;
}