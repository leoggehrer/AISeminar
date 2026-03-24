import { Component, VERSION } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '@app-services/auth.service';

export class DashboardCard {
  visible: boolean = true;
  title: string;
  text: string;
  type: string;
  bg: string;
  icon: string;
  constructor(visible: boolean = true, title: string, text: string, type: string, bg: string, icon: string) {
    this.visible = visible;
    this.title = title;
    this.text = text;
    this.type = type;
    this.bg = bg;
    this.icon = icon;
  }
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {
  public readonly appVersion = VERSION.full;

  public publicCards: DashboardCard[] = [
//    { visible: true, title: 'DASHBOARD.CARDS.BLOG_ENTRY_TITLE', text: 'DASHBOARD.CARDS.BLOG_ENTRY_TEXT', type: '/blog-entries', bg: 'bg-info text-white', icon: 'bi-journal-text' },
  ];

  public authCards: DashboardCard[] = [
  ];

  constructor(
    private authService: AuthService) {

  }

  public get isLoginRequired(): boolean {
    return this.authService.isLoginRequired;
  }

  public get isLoggedIn(): boolean {
    return this.authService.isLoggedIn;
  }

  public get visiblePublicCards(): DashboardCard[] {
    return this.publicCards.filter(c => c.visible);
  }

  public get visibleAuthCards(): DashboardCard[] {
    return this.authCards.filter(c => c.visible);
  }

  public logout() {
    this.authService.logout();
  }
}
