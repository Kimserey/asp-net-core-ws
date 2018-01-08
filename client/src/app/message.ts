import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs/Rx';
import { WebsocketService } from './websocket';
import { Message } from './model';
import { environment } from '../environments/environment';

@Injectable()
export class MessageService {
  public messages$: Observable<Message>;

  constructor(private webSocketService: WebsocketService) {
    this.messages$ = webSocketService
      .connect(environment.ws)
      .do(ev => console.log(ev))
      .map(ev => ev.data);
  }

  send(content) {
    this.webSocketService.send({ content, date: new Date() });
  }
}
