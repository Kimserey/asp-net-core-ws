import { AppComponent } from './app.component';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { MessageService } from './message';
import { NgModule } from '@angular/core';
import { WebsocketService } from './websocket';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [
    MessageService,
    WebsocketService
],
  bootstrap: [AppComponent]
})
export class AppModule { }
