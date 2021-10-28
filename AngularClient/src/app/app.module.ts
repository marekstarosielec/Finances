import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ToastrModule } from "ngx-toastr";

import { SidebarModule } from './sidebar/sidebar.module';
import { FooterModule } from './shared/footer/footer.module';
import { NavbarModule} from './shared/navbar/navbar.module';
import { FixedPluginModule} from './shared/fixedplugin/fixedplugin.module';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { AppRoutes } from './app.routing';

import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';
import { ApiModule } from "./api/generated/api.module";
import { CloseDatasetComponent } from "./pages/close-dataset/close-dataset.component";
import { DashboardComponent } from "./pages/dashboard/dashboard.component";
import { IconsComponent } from "./pages/icons/icons.component";
import { MapsComponent } from "./pages/maps/maps.component";
import { NotificationsComponent } from "./pages/notifications/notifications.component";
import { OpenDatasetComponent } from "./pages/open-dataset/open-dataset.component";
import { TransactionsComponent } from "./pages/transactions/transactions.component";
import { TypographyComponent } from "./pages/typography/typography.component";
import { UserComponent } from "./pages/user/user.component";
import { LoadingIndicatorComponent } from "./shared/loading-indicator/loading-indicator.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";

@NgModule({
  declarations: [
    AppComponent,
    AdminLayoutComponent,
    DashboardComponent,
    UserComponent,
    TransactionsComponent,
    OpenDatasetComponent,
    CloseDatasetComponent,
    TypographyComponent,
    IconsComponent,
    MapsComponent,
    NotificationsComponent,
    LoadingIndicatorComponent
  ],
  imports: [
    BrowserAnimationsModule,
    RouterModule.forRoot(AppRoutes,{
      useHash: true
    }),
    SidebarModule,
    NavbarModule,
    ToastrModule.forRoot(),
    FooterModule,
    FixedPluginModule,
    HttpClientModule,
    ApiModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
