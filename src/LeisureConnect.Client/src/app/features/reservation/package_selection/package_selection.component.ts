import { Component, input, model, output, computed, OnChanges, SimpleChanges } from '@angular/core';
import { AdvertisedPackage } from '../../../core/models/advertised_package.model';

interface PackageType {
  id: 'all' | 'standard' | 'custom';
  name: string;
}

@Component({
  selector: 'app-package-selection',
  templateUrl: './package_selection.component.html',
  standalone: true,
})
export class PackageSelectionComponent implements OnChanges {
  packages = input<AdvertisedPackage[]>([]);
  isLoading = input<boolean>(false);
  hotelId = input<number | null>(null);
  startDate = input<Date | null>(null);
  endDate = input<Date | null>(null);
  
  packageSelected = output<number>();
  
  filteredPackages: AdvertisedPackage[] = [];
  searchText = '';
  selectedType: 'all' | 'standard' | 'custom' = 'all';
  
  readonly packageTypes: PackageType[] = [
    { id: 'all', name: 'All Packages' },
    { id: 'standard', name: 'Standard Packages' },
    { id: 'custom', name: 'Custom Packages' }
  ];
  
  // Computed property for stay length
  stayLength = computed(() => {
    if (!this.startDate() || !this.endDate()) {
      return 0;
    }
    
    const diffTime = this.endDate()!.getTime() - this.startDate()!.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  });
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['packages']) {
      this.applyFilters();
    }
  }
  
  selectPackage(packageId: number): void {
    this.packageSelected.emit(packageId);
  }
  
  onSearchChange(): void {
    this.applyFilters();
  }
  
  onTypeChange(): void {
    this.applyFilters();
  }
  
  applyFilters(): void {
    const lowerSearchText = this.searchText.toLowerCase();
    const hasSearchText = this.searchText !== '';
    
    this.filteredPackages = this.packages().filter(pkg => {
      const searchMatch = !hasSearchText || 
        pkg.name.toLowerCase().includes(lowerSearchText) ||
        pkg.description.toLowerCase().includes(lowerSearchText);
      
      const typeMatch = this.selectedType === 'all' || 
        (this.selectedType === 'standard' && pkg.isStandardPackage) ||
        (this.selectedType === 'custom' && !pkg.isStandardPackage);
      
      return searchMatch && typeMatch;
    });
  }
  
  getTotalPrice(basePrice: number): number {
    return basePrice * this.stayLength();
  }
}