import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.dev";
import { HttpClient } from "@angular/common/http";
import { ReservationRequest } from "../models/reservation.model";
import { ReservationResponse } from "../models/reservation_response.model";
import { Observable } from "rxjs";

@Injectable({
    providedIn: "root",
})
export class ReservationService {
    private readonly apiUrl: string = `${environment.apiUrl}/reservation`;
    
    constructor(private http: HttpClient) { }

    createReservation(request: ReservationRequest): Observable<ReservationResponse> {
        if (!request) {
            throw new Error("Reservation request cannot be null or undefined.");
        }

        return this.http.post<ReservationResponse>(this.apiUrl, request);
    }
        
    getReservation(id: number): Observable<ReservationResponse> {
        if (typeof id !== "number" || id <= 0) {
            throw new Error("Invalid reservation ID.");
        }

        return this.http.get<ReservationResponse>(`${this.apiUrl}/${id}`);
    }
}