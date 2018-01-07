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
      .map((response: MessageEvent): Message => {
        return {
          content: response.data,
          type: ''
        };
      });
  }

  send(content) {
    this.webSocketService
      .connect(environment.ws)
      .next(<MessageEvent> {
        data: content
      });
  }
}
