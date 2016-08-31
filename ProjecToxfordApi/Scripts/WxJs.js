wx.config({
    debug: true, //false  true开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
    appId: appIdstr,
    timestamp: timestampstr,
    nonceStr: nonceStrstr,
    signature: signaturestr,
    jsApiList: [
        'checkJsApi',
        'onMenuShareTimeline',
        'onMenuShareAppMessage',
        'onMenuShareQQ',
        'onMenuShareWeibo',
        'hideMenuItems',
        'showMenuItems',
        'hideAllNonBaseMenuItem',
        'showAllNonBaseMenuItem',
        'translateVoice',
        'startRecord',
        'stopRecord',
        'onRecordEnd',
        'playVoice',
        'pauseVoice',
        'stopVoice',
        'uploadVoice',
        'downloadVoice',
        'chooseImage',
        'previewImage',
        'uploadImage',
        'downloadImage',
        'getNetworkType',
        'openLocation',
        'getLocation',
        'hideOptionMenu',
        'showOptionMenu',
        'closeWindow',
        'scanQRCode',
        'chooseWXPay',
        'openProductSpecificView',
        'addCard',
        'chooseCard',
        'openCard'
    ]
});
wx.ready(function () {
});

//上传图片uploadimgs
function uploadimgs() {

    wx.chooseImage({
        count: 1, // 默认9
        sizeType: ['original', 'compressed'], // 可以指定是原图还是压缩图，默认二者都有
        sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
        success: function (res) {
            var localIds = res.localIds; // 返回选定照片的本地ID列表，localId可以作为img标签的src属性显示图片      
            SeaveWx(localIds);
        }, fail: function () { }
    });
}
//保存到服务器
function SeaveWx(localIds) {
    // 上传图片接口   
    wx.uploadImage({
        localId: localIds.toString(), // 需要上传的图片的本地ID，由chooseImage接口获得
        isShowProgressTips: 1, // 默认为1，显示进度提示
        success: function (res) {
            var serverId = res.serverId; // 返回图片的服务器端ID  serverId 即 media_id           
            //  备注：上传图片有效期3天，可用微信多媒体接口下载图片到自己的服务器，此处获得的 serverId 即 media_id，参考文档 http://mp.weixin.qq.com/wiki/12/58bfcfabbd501c7cd77c19bd9cfa8354.html 目前多媒体文件下载接口的频率限制为10000次/天，如需要调高频率，请邮件weixin-open@qq.com,邮件主题为【申请多媒体接口调用量】，请对你的项目进行简单描述，附上产品体验链接，并对用户量和使用量进行说明。   
            SeaveLocal(localIds,serverId);
        }, fail: function () { alert("上传服务器图片失败"); }
    });
}
//种类（1.1：娃像谁 3人照片 1张；1.2：娃像谁 单人照 3张 ；2.1：夫妻相 双人照 1张；2.2夫妻相 2张）
//设置图片显示在页面上
function SeaveLocal(localIds, serverId) {
    if (localIds.length > 0 && serverId.length > 0) {     
        $('#' + BtnId + '').html("<img class=\"weui_media_appmsg_thumb\" src=\"" + localIds + "\" data-serverid=" + serverId + " >");
        BtnId = "";
        $.get('home/downtemp', { serverId: serverId }, function (data) {
            if (data.isLock == 1) {
               // alert(data.ImgUrl)
            }
        });
        } else {
            alert("上传失败");
        }
    //});
}

//从服务器下载图片
function downLoad(serverId) {
    // 下载图片接口
    wx.downloadImage({
        serverId: serverId.toString(), // 需要下载的图片的服务器端ID，由uploadImage接口获得
        isShowProgressTips: 1, // 默认为1，显示进度提示
        success: function (res) {
            var localId = res.localId; // 返回图片下载后的本地ID
            alert("下载图片接口" + localId);
            s4 = localId;
            alert("s4=" + s4);
            $("#ddd").html(localId)

            $("#dimg").attr("src", s4)

        }, fail: function () {
            alert("下载图片接口图片失败");
        }
    });
}

