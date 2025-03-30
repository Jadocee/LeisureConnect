export interface ReservationItem {
    packageId?: number;
    serviceItemId?: number;
    quantity: number;
    startDate: string;
    endDate: string;
    specialRequests?: string;
}