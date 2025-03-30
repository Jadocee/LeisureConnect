import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment.dev";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { PaymentMethod } from "../models/payment_method.model";

@Injectable({
    providedIn: "root",
})
export class PaymentMethodService {
    private readonly apiUrl: string = `${environment.apiUrl}/paymentMethod`;

    constructor(private http: HttpClient) { }

    getAllPaymentMethods(): Observable<PaymentMethod[]> {
        return this.http.get<PaymentMethod[]>(this.apiUrl);
    }
}