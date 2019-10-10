import {Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute} from "@angular/router";
import {Notifications} from "../notifications";
import {Title} from "@angular/platform-browser";

declare var Talk, talkSession: any;

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  loadingText: string = "Loading chat...";
  otherId? : number;

  constructor(private httpClient: HttpClient, @Inject('BASE_URL') private baseUrl: string, private route: ActivatedRoute, private title: Title) {
    this.otherId = parseInt(this.route.snapshot.paramMap.get('id'));
    if (isNaN(this.otherId))
      this.otherId = null;
    this.title.setTitle("Chat - NotAmazon.com");
  }

  ngOnInit(): void {
    this.httpClient.get(this.baseUrl + 'api/Account/StartChat', {params: {id: String(this.otherId)}}).subscribe(result => {
      this.startChat(result);
    }, error => {
      console.error(error);
      const errorMsg = error["error"]["error"];
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
      let me = new Talk.User({
        id: result["userId"],
        name: result["userName"],
        email: result["userEmail"],
      });


      (<any>window).talkSession = new Talk.Session({
        appId: "tFj0WqN6",
        me: me,
        signature: result["signature"]
      });

      let inbox;
      // Open specific conversation if server says to
      if (result["otherId"] != "") {
        let other = new Talk.User({
          id: result["otherId"],
          name: result["otherName"],
        });
        let conversation = talkSession.getOrCreateConversation(Talk.oneOnOneId(me, other));
        conversation.setParticipant(me);
        conversation.setParticipant(other);
        inbox = talkSession.createInbox({selected: conversation});
      } else {
        inbox = talkSession.createInbox();
      }

      inbox.mount(document.getElementById("talkjs-container"));
    });
  }

}
