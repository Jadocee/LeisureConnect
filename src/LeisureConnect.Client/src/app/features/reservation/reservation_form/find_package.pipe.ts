import { Pipe, PipeTransform } from '@angular/core';
import { AdvertisedPackage } from '../../../core/models/advertised_package.model';

@Pipe({
    name: 'findPackage',
    standalone: true,
    pure: true
})
export class FindPackagePipe implements PipeTransform {
    transform(packages: AdvertisedPackage[] | null, packageId: number | null): AdvertisedPackage | null {
        if (!packages || !packageId) {
            return null;
        }

        return packages.find(pkg => pkg.packageId === packageId) || null;
    }
}