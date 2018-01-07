import { Component, OnInit } from '@angular/core';
import { environment } from '../environments/environment';
import { MessageService } from './message';
import { Message } from './model';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'app';
  wsUrl = environment.ws;
  messages$: Observable<Message>;
  data: string;

  constructor(private messageService: MessageService) { }

  ngOnInit() {
    this.messages$ = this.messageService.messages$;
  }

  send() {
    this.messageService.send(this.data);
  }
}
