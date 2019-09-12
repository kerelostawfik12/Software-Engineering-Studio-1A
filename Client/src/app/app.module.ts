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
import {BrowseItemsComponent} from './browse-items/browse-items.component';

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
    BrowseItemsComponent,

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
      {path: 'item/:id', component: ItemPageComponent},
      {path: 'new-item-form', component: NewItemFormComponent},
      {path: 'item-view', component: ItemViewComponent},
      {path: 'browse', component: BrowseItemsComponent},
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {
}
