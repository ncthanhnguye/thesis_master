import {
  Component,
  OnInit,
  ViewChild,
  TemplateRef,
  Input,
  OnDestroy,
  AfterViewInit,
  Inject,
  PLATFORM_ID,
} from '@angular/core';
import videojs from 'video.js';
import { MatDialog } from '@angular/material/dialog';
import { isPlatformBrowser, isPlatformServer } from '@angular/common';
import { AppConsts } from '../services/app.consts';
@Component({
  selector: 'app-videojs',
  templateUrl: './videojs.component.html',
  // styleUrls: ['./videojs.component.scss']
})
export class VideojsComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('video') video: any;
  @ViewChild('modal') customTemplate: TemplateRef<any>;
  @ViewChild('video_default') video_default: any;
  @ViewChild('video_box') video_box: any;
  @ViewChild('video_error') video_error: any;
  @Input('url') UrlVideo: any = '';
  @Input('customHTML') customHTML: String = '';
  @Input('zoom') zoom: boolean;
  @Input('controls') controls: boolean;
  @Input('bigPlayButton') isBigPlayButton: boolean;
  @Input('picInPic') pip: boolean;
  urlData: String;
  player: videojs.Player;
  expandButtonDefault =
    "<span class='zoom-view buttonZoom'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>";
  isBrowser:any;
  constructor(public dialog: MatDialog, @Inject(PLATFORM_ID) platformId: Object) {
    this.isBrowser = isPlatformBrowser(platformId);
  }

  ngOnInit(): void {
    this.urlData = this.UrlVideo;
    if (this.isBigPlayButton != false) {
      this.isBigPlayButton = true;
    }
    if (this.controls != false) {
      this.controls = true;
    }
    if (this.pip != false) {
      this.pip = true;
    }
  }

  ngAfterViewInit(): void {
    if(this.isBrowser){
      if (this.video) {
        this.player = videojs(this.video.nativeElement, {
          bigPlayButton: this.isBigPlayButton,
          controls: this.controls,
          controlBar: { pictureInPictureToggle: this.pip },
          sources: {
            src: this.UrlVideo,
          },
        });
        if (window.innerWidth > 576) {
          var myButton = this.player.controlBar.addChild('button');
          var myButtonDom = myButton.el();
          myButtonDom.innerHTML = this.expandButtonDefault;
          myButtonDom.title = 'Expand';
          myButtonDom.className = myButtonDom.className + ' buttonZoom';
          this.player.on('fullscreenchange', () => {
            if (!this.player.isFullscreen()) {
              this.player.pause();
            }
          });
        }
      }
    }
  }

  ngOnDestroy(): void {
    if (this.player) {
      this.player.dispose();
    }
  }

  openDialog(event) {
    let openDialog = this.zoom;
    if (!this.zoom) {
      openDialog = event.target.className.includes('buttonZoom');
    }
    let currentVideo;
    let source;
    // if (this.zoom) {
    //   currentVideo = this.video_default.nativeElement;
    // } else {
    //   currentVideo = this.video.nativeElement;
    // }
    currentVideo = this.video.nativeElement;
    source = currentVideo.currentSrc;
    if (openDialog) {
      if (window.innerWidth <= 576) {
        currentVideo.requestFullscreen();
      } else {
        if (!this.zoom) {
          //pause video current
          this.player = videojs(currentVideo);
          this.player.pause();
        }
        const dialogRef = this.dialog.open(this.customTemplate, {
          data: source,
        });
        //open modal and play video in modal
        this.player = videojs(document.getElementById('video-player-zoom'), {
          controls: true,
          sources: {
            src: source,
          },
        });
        this.player.on('fullscreenchange', () => {
          if (!this.player.isFullscreen()) {
            this.player.dispose();
            dialogRef.close();
          }
        });
        this.player.play();
      }
    }
  }
  videoError(){
    this.video_box.nativeElement.style.display="none";
    this.video_error.nativeElement.style.display="block";
    this.video_error.nativeElement.src = AppConsts.DEFAULT_IMAGE;
  }
}
