import { Component, OnInit } from '@angular/core';
import { environment } from '../environments/environment';
import { MessageService } from './message';
import { Message } from './model';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'app';
  wsUrl = environment.ws;
  messages: Message[] = [];
  data: string;
  sub: Subscription;

  constructor(private messageService: MessageService) { }

  ngOnInit() {
    this.sub = this.messageService.messages$
      .subscribe(c => {
        this.messages.push(c);
      });
  }

  send() {
    this.messageService.send(this.data);
  }
}
