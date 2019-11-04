import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {CounterComponent} from './counter/counter.component';
import {FetchDataComponent} from './fetch-data/fetch-data.component';
import {CartComponent} from './cart/cart.component';
import {ComponentTestZoneComponent} from './component-test-zone/component-test-zone.component';
import {CartItemComponent} from './cart-item/cart-item.component';
import {NewItemFormComponent} from './new-item-form/new-item-form.component';
import {NavbarComponent} from './navbar/navbar.component';
import {RegisterComponent} from './register/register.component';
import {ItemPageComponent} from "./item-page/item-page.component";
import {ItemViewComponent} from "./item-view/item-view.component";
import {ViewCustomersComponent} from './view-customers/view-customers.component';
import {SearchItemsComponent} from './search-items/search-items.component';
import {LoginComponent} from './login/login.component';
import {ProfilePageComponent} from './profile-page/profile-page.component';
import {AdminComponent} from './admin/admin.component';
import {ViewTransactionsComponent} from './view-transactions/view-transactions.component';
import {SmallItemListingComponent} from "./small-item-listing/small-item-listing.component";
import {ViewSellersComponent} from "./view-sellers/view-sellers.component";
import {SellerRegisterComponent} from "./seller-register/seller-register.component";
import {AdminRegisterComponent} from "./admin-register/admin-register.component";
import {LongItemViewComponent} from "./long-item-view/long-item-view.component";
import {PaypalComponent} from "./paypal/paypal.component";
import {ChatComponent} from './chat/chat.component';
import {ThankYouComponent} from './thank-you/thank-you.component';
import {RecommendedItemsComponent} from "./recommended-items/recommended-items.component";
import {HelpComponent} from './help/help.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    CartComponent,
    ComponentTestZoneComponent,
    CartItemComponent,
    NewItemFormComponent,
    NavbarComponent,
    RegisterComponent,
    ItemPageComponent,
    ItemViewComponent,
    ViewCustomersComponent,
    ViewSellersComponent,
    SearchItemsComponent,
    LoginComponent,
    LongItemViewComponent,
    ProfilePageComponent,
    AdminComponent,
    ViewTransactionsComponent,
    SmallItemListingComponent,
    SellerRegisterComponent,
    AdminRegisterComponent,
    PaypalComponent,
    ChatComponent,
    ThankYouComponent,
    RecommendedItemsComponent,
    HelpComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
      {path: 'counter', component: CounterComponent},
      {path: 'fetch-data', component: FetchDataComponent},
      {path: 'cart', component: CartComponent},
      {path: 'component-test-zone', component: ComponentTestZoneComponent},
      {path: 'register', component: RegisterComponent},
      {path: 'login', component: LoginComponent},
      {path: 'item/:id', component: ItemPageComponent},
      {path: 'new-item-form', component: NewItemFormComponent},
      {path: 'new-item-form', component: AdminComponent},
      {path: 'item-view', component: ItemViewComponent},
      {path: 'search/:query', component: SearchItemsComponent},
      {path: 'admin', component: AdminComponent},
      {path: 'profile', component: ProfilePageComponent},
      {path: 'view-transactions', component: ViewTransactionsComponent},
      {path: 'view-customers', component: ViewCustomersComponent},
      {path: 'view-sellers', component: ViewSellersComponent},
      {path: 'register-seller', component: SellerRegisterComponent},
      {path: 'register-admin', component: AdminRegisterComponent},
      {path: 'chat/:id', component: ChatComponent},
      {path: 'chat', component: ChatComponent},
      {path: 'help', component: HelpComponent},
      {path: 'thank-you', component: ThankYouComponent},
      {path: 'thank-you/:loot', component: ThankYouComponent},
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
}
