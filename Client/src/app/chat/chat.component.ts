import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {Notifications} from "../notifications";

declare var Talk, talkSession: any;

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  loadingText: string = "Loading chat...";
  otherId? : number;
  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string, private route: ActivatedRoute) {
    this.otherId = parseInt(this.route.snapshot.paramMap.get('id'));
    if (isNaN(this.otherId))
      this.otherId = null;
    console.log(this.otherId);
  }

  ngOnInit(): void {
    this.httpClient.get(this.baseUrl + 'api/Account/StartChat', {params: {id: String(this.otherId)}}).subscribe(result => {
      this.startChat(result);
    }, error => {
      console.error(error)
      var errorMsg = error["error"]["error"];
      Notifications.error(errorMsg);
      this.loadingText = errorMsg;
    });
  }

  startChat(result: any) {
    (function(t:any,a:any,l:any,k,j,s){
      s=a.createElement('script');s.async=1;s.src="https://cdn.talkjs.com/talk.js";a.head.appendChild(s)
      ;k=t.Promise;t.Talk={v:2,ready:{then:function(f){if(k)return new k(function(r,e){l.push([f,r,e])});l
            .push([f])},catch:function(){return k&&new k()},c:l}};})(window,document,[]);
    Talk.ready.then(function() {
      var me = new Talk.User({
        id: result["userId"],
        name: result["userName"],
        email: result["userEmail"],
        role: result["userRole"]
      });
      (<any>window).talkSession = new Talk.Session({
        appId: "tFj0WqN6",
        me: me,

        signature: result["signature"]
      });



      var inbox = talkSession.createInbox();
      inbox.mount(document.getElementById("talkjs-container"));
    });
  }

}
