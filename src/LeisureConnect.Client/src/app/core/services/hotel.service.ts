import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.dev";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { Hotel } from "../models/hotel.model";

@Injectable({
    providedIn: "root",
})
export class HotelService {
    private readonly apiUrl: string = `${environment.apiUrl}/hotel`;

    constructor(private http: HttpClient) { }

    getAllHotels(): Observable<Hotel[]> {
        return this.http.get<Hotel[]>(this.apiUrl);
    }

    getHotel(id: number): Observable<Hotel> {
        if (typeof id !== "number" || id <= 0) {
            throw new Error("Invalid hotel ID.");
        }

        return this.http.get<Hotel>(`${this.apiUrl}/${id}`);
    }
}