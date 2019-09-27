function WeixinShare(title, desc, link, imgUrl, jsApiList) {
    $(document).ready(function () {
        //分享链接处理
        if (link.indexOf("#") > 0) {
            link = link.substring(0, link.indexOf("#"));
        }
        //通过Ajax获取签名信息，不影响主页面的加载逻辑
        $.ajax({
            url: "/apiaction/Weixin/GetSignature",
            data: "url=" + encodeURIComponent(link),
            type: "GET",
            dataType: "json",
            success: function (data) {
                //注入配置信息
                wx.config({
                    debug: false,// 开启调试模式,调用的所有api的返回值会在客户端alert出来
                    appId: data.appId,// 必填，公众号的唯一标识
                    timestamp: parseInt(data.timestamp),// 必填，生成签名的时间戳
                    nonceStr: data.nonceStr,// 必填，生成签名的随机串
                    signature: data.signature,// 必填，签名
                    jsApiList: ['updateAppMessageShareData', 'updateTimelineShareData']// 必填，需要使用的JS接口列表
                });

                //config验证成功后调用微信接口
                wx.ready(function () {
                    //分享给朋友
                    wx.updateAppMessageShareData({
                        title: title, desc: desc, link: link, imgUrl: imgUrl,
                        success: function () { }
                    });
                    //分享到朋友圈
                    wx.updateTimelineShareData({
                        title: title, desc: desc, link: link, imgUrl: imgUrl,
                        success: function () { }
                    });
                });
                wx.error(function (res) {
                    console.log(res);
                });
            }
        });
    });
}