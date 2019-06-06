import { Component, OnInit, OnDestroy } from '@angular/core';
import { ServersService } from './services/servers.service';
import { VirtualServer } from './models/virtualServer';
import * as _ from 'lodash';
import * as moment from 'moment';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

    servers: VirtualServer[];
    private timer: any;
    private usageTime: string = "00:00:00";
    private currentDateTime: Date = new Date();
    private pageLoadDateTime: Date = new Date();
    private hubConnection: HubConnection;

    constructor(private serversService: ServersService) {

    }

    ngOnInit(): void {
        this.serversService.getServers()
            .subscribe(result => {
                this.servers = result;
                this.reorderServers();
                this.calculateUsageTime();
            });

        this.timer = setInterval(() => {
            this.calculateUsageTime();
            this.currentDateTime = this.now;
        }, 1000); // every second

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/api/serversHub")
            .build();

        this.hubConnection.start().catch(err => console.error(err.toString()));

        this.hubConnection.on('SendServerAdded', (server: VirtualServer) => {
            this.servers.push(server);
            this.reorderServers();
        });

        this.hubConnection.on('SendServerChanged', (s: VirtualServer) => {
            let server = this.servers.find(ss => ss.virtualServerId == s.virtualServerId);
            if (server) {
                _.merge(server, s);
            }
        });

        this.hubConnection.on('SendServersRemoved', (servers: VirtualServer[]) => {
            servers.forEach(s => {
                let server = this.servers.find(ss => ss.virtualServerId == s.virtualServerId);
                if (server) {
                    _.merge(server, s);
                }
                else {
                    this.servers.push(s);
                }
            });

            this.reorderServers();
        });
    }

    ngOnDestroy() {
        clearInterval(this.timer);
    }

    
    get current(): Date {
        return this.currentDateTime;
    }

    get pageLoad(): Date {
        // показывается текущее время как время загрузки страницы (а не таймер с увеличением секунд) чтобы можно было понять момент загрузки списка серверов
        return this.pageLoadDateTime;
    }
    
    get totalUsageTime(): string {
        return this.usageTime;
    }

    add() {
        this.serversService.addServer()
            .subscribe();
    }

    selectForRemove(data: VirtualServer) {
        this.serversService.selectForRemove(data.virtualServerId, data.selectedForRemove)
            .subscribe();
    }

    removeSelected() {
        this.serversService.removeSelected()
            .subscribe();
    }

    get isAnySelectedForRemove(): boolean {
        return _.some(this.servers, s => s.selectedForRemove);
    }

    private get now() {
        return new Date();
    }

    private reorderServers() {
        this.servers = _.orderBy(this.servers, s => s.createDateTime);
    }

    private calculateUsageTime() {
        if (this.servers && this.servers.length > 0) {
            let servers: VirtualServer[] = _.orderBy(this.servers, s => s.createDateTime);
            let prev = servers[0];
            let start = this.getStart(prev);
            let end = this.getEnd(prev);
            let time = 0;

            for (let i = 1; i < servers.length; i++) {
                let next = servers[i];
                // если дата создания сервер больше, чем период -- сохраняем продолжительность
                if (this.getStart(next) > end) {
                    time += end - start;
                    start = this.getStart(next);
                    end = this.getEnd(next);
                }
                // иначе, если дата закрытия больше, чем конец периода -- сдвигаем дату закрытия
                else if (this.getEnd(next) > end) {
                    end = this.getEnd(next);
                }

                prev = next;
            }

            time += end - start;
            this.usageTime = this.getDurationString(time);
        }
    }

    private pad(time: number) {
        return time <= 9 ? `0${time}` : time;
    }

    private getStart(server: VirtualServer): number {
        return moment(server.createDateTime).valueOf();
    }

    private getEnd(server: VirtualServer): number {
        return moment(server.removeDateTime || this.now).valueOf();
    }
    
    private getDurationString(time: number): string {
        time = time || 0;
        let seconds = Math.floor(time / 1000 % 60);
        let minutes = Math.floor(time / 1000 / 60 % 60);
        let hours = Math.floor(time / (1000 * 60 * 60));

        return `${this.pad(hours)}:${this.pad(minutes)}:${this.pad(seconds)}`;
    }
}
