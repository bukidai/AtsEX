const url = location.href;
const pathName = location.pathname;
const title = document.title;

setUrl("twitter", "https://twitter.com/share?url=" + url + "&via=atF9045&related=atF9045&text=" + title);
setUrl("facebook", "http://www.facebook.com/share.php?u=" + url);
setUrl("hatena", "http://b.hatena.ne.jp/add?mode=confirm&url=" + url + "&title=" + title);

function setUrl(snsName, snsUrl) {
	const elements = document.getElementsByClassName("footer-sns-" + snsName);
	switch (elements.length) {
		case 0:
			console.error("Cannot find the class 'footer-sns-" + snsName + "'");
			return;

		case 1:
			elements[0].href = snsUrl;
			elements[0].addEventListener("click", e => onShare(snsName));
			break;

		default:
			Array.from(elements).forEach(element => {
				element.url = snsUrl;
				element.addEventListener("click", e => onShare(snsName));
			});
			break;
	}
}

function onShare(snsName) {
	gtag("event", "share", { "event_category": "engagement", "event_label": snsName });
}