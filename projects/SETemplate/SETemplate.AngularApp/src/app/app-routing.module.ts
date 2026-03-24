import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { LoginComponent } from './pages/auth/login/login.component';
import { LandingComponent } from './pages/landing/landing.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';

export const routes: Routes = [
  // Landing Page
  { path: '', component: LandingComponent },
  
  // Öffentlicher Login-Bereich
  { path: 'auth/login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },

  /* Entity-Routen
  {
    path: 'blog-entries',
    loadComponent: () => import('./pages/entities/app/blog-entry-list.component')
      .then(m => m.BlogEntryListComponent),
    canActivate: [AuthGuard],
    title: 'Blog Entries'
  },
  */
 
  // Fallback bei ungültiger URL
  { path: '**', redirectTo: '' }
];
