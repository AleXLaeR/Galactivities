import { makeAutoObservable, runInAction } from 'mobx';
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';

import { Comment } from '@models/index';
import { store } from '@store/index';

export default class CommentStore {
  comments: Comment[] = [];

  hubConnection: HubConnection | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  public createHubConnection = async (activityId?: string) => {
    if (activityId) {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(`${import.meta.env.VITE_API_CHAT_URL}?activityId=${activityId}`, {
          skipNegotiation: true,
          transport: HttpTransportType.WebSockets,
          accessTokenFactory: () => store.userStore.user?.token ?? '',
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      await this.hubConnection
        .start()
        .catch((error) => console.log(`Error establishing hub connection: ${error}`));

      this.hubConnection.on('LoadComments', (comments: Comment[]) => {
        // console.log(comments);
        runInAction(() => {
          comments.forEach((comment) => {
            comment.createdAt = new Date(comment.createdAt);
          });
          this.comments = comments.sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime());
        });
      });

      this.hubConnection.on('ReceiveComment', (comment: Comment) => {
        // console.log(comment);
        runInAction(() => {
          comment.createdAt = new Date(comment.createdAt);
          this.comments.unshift(comment);
        });
      });
    }
  };

  public stopHubConnection = () => {
    this.hubConnection
      ?.stop()
      .catch((error) => console.log(`Error stopping hub connection: ${error}`));
  };

  public clearComments = () => {
    this.comments = [];
    this.stopHubConnection();
  };

  public addComment = async (values: any) => {
    values.activityId = store.activityStore.selectedActivity?.id;

    try {
      await this.hubConnection?.invoke('SendComment', values);
    } catch (error: any) {
      console.log(error);
    }
  };
}
