import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from './services/auth.service';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, NgbDropdownModule, TranslateModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  public title = 'SETemplate';
  public currentLanguage = 'de';
  public currentTheme: 'light' | 'dark' = 'light';
  public isMenuCollapsed = true;

  public get isLoginRequired(): boolean {
    return this.authService.isLoginRequired;
  }
  public get isLoggedIn(): boolean {
    return this.authService.isLoggedIn;
  }
  public get userName(): string {
    return this.authService.user?.name || '';
  }

  constructor(
    private router: Router,
    private authService: AuthService,
    private translateService: TranslateService) {

    // Sprache beim ersten Laden sofort setzen
    const savedLang = localStorage.getItem('language') || 'de';
    this.currentLanguage = savedLang;
    this.translateService.setDefaultLang('de');
    this.translateService.use(savedLang);

    // Theme aus localStorage laden oder Standard verwenden
    const savedTheme = this.readStoredTheme();
    this.currentTheme = savedTheme || 'light';
    this.applyTheme(this.currentTheme);

    // Auf Sprachwechsel reagieren
    this.translateService.onLangChange.subscribe((event) => {
      this.currentLanguage = event.lang;
    });
  }
  
  public switchLanguage(language: string) {
    this.translateService.use(language).subscribe(() => {
      this.currentLanguage = language;
      localStorage.setItem('language', language);
    });
  }

  public logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }

  public toggleTheme() {
    this.currentTheme = this.currentTheme === 'light' ? 'dark' : 'light';
    this.applyTheme(this.currentTheme);
    this.writeStoredTheme(this.currentTheme);
  }

  private applyTheme(theme: 'light' | 'dark') {
    document.documentElement.setAttribute('data-bs-theme', theme);
  }

  private readStoredTheme(): 'light' | 'dark' | null {
    try {
      const storedTheme = localStorage.getItem('theme');
      if (storedTheme === 'light' || storedTheme === 'dark') {
        return storedTheme;
      }
    }
    catch {
    }
    return null;
  }

  private writeStoredTheme(theme: 'light' | 'dark'): void {
    try {
      localStorage.setItem('theme', theme);
    }
    catch {
    }
  }
}
