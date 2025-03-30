import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.dev";
import { Observable } from "rxjs";
import { AdvertisedPackage } from "../models/advertised_package.model";
import { HttpClient, HttpParams } from "@angular/common/http";

@Injectable({
    providedIn: "root",
})
export class AdvertisedPackageService {
    private readonly apiUrl: string = `${environment.apiUrl}/advertisedPackage`;

    constructor(private http: HttpClient) { }

    getAvailableAdvertisedPackages(hotelId: number, startDate: Date, endDate: Date): Observable<AdvertisedPackage[]> {
        if (typeof hotelId !== "number" || hotelId <= 0) {
            throw new Error("Invalid hotel ID.");
        }

        if (!(startDate instanceof Date) || !(endDate instanceof Date)) {
            throw new Error("Invalid date range.");
        }

        if (startDate >= endDate) {
            throw new Error("Start date must be before end date.");
        }

        let params: HttpParams = new HttpParams()
            .set("hotelId", hotelId.toString())
            .set("startDate", startDate.toISOString().split("T")[0]) // Format date to YYYY-MM-DD
            .set("endDate", endDate.toISOString().split("T")[0]); // Format date to YYYY-MM-DD

        return this.http.get<AdvertisedPackage[]>(`${this.apiUrl}/available`, { params });
    }

    getAdvertisedPackage(id: number): Observable<AdvertisedPackage> {
        if (typeof id !== "number" || id <= 0) {
            throw new Error("Invalid package ID.");
        }

        return this.http.get<AdvertisedPackage>(`${this.apiUrl}/${id}`);
    }
}