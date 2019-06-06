import { Component, OnInit, OnDestroy } from '@angular/core';
import { ServersService } from './services/servers.service';
import { VirtualServer } from './models/virtualServer';
import * as _ from 'lodash';
import * as moment from 'moment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {

    servers: VirtualServer[];
    private timer: any;
    private usageTime: string;
    private currentDateTime: Date = new Date();
    private pageLoadDateTime: Date = new Date();

    constructor(private serversService: ServersService) {

    }

    ngOnInit(): void {
        this.serversService.getServers()
            .subscribe(result => {
                this.servers = result;
                this.calculateUsageTime();
            });

        this.timer = setInterval(() => {
            this.calculateUsageTime();
            this.currentDateTime = new Date();
        }, 1000); // every second

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
            .subscribe(srv => this.appendServer(srv));
    }

    selectForRemove() {
        let ids = this.servers.filter(s => s.selected).map(s => s.virtualServerId);

        this.serversService.selectForRemove(ids)
            .subscribe(() => {
                this.servers.filter(s => s.selected).forEach(s => { s.selected = false; s.selectedForRemove = true; });
            });
    }

    get isAnySelected(): boolean {
        return _.some(this.servers, s => s.selected);
    }

    private appendServer(srv: VirtualServer) {
        this.servers = _.orderBy(this.servers.concat([srv]), s => s.createDateTime);
    }

    private calculateUsageTime() {
        if (this.servers) {
            let min = _.minBy(this.servers, s => s.createDateTime);

            if (min) {
                this.usageTime = this.getDuration(min.createDateTime, new Date());
            }
        }
    }

    private pad(time: number) {
        return time <= 9 ? `0${time}` : time;
    }

    private getDuration(start: Date, end: Date) {
        let time = moment(end).valueOf() - moment(start).valueOf();
        let seconds = Math.floor(time / 1000 % 60);
        let minutes = Math.floor(time / 1000 / 60 % 60);
        let hours = Math.floor(time / (1000 * 60 * 60));

        return `${this.pad(hours)}:${this.pad(minutes)}:${this.pad(seconds)}`;
    }
}
