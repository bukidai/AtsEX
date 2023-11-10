let _result = null;
const getLatestRelease = new Promise(resolve => {
	if (_result == null) {
		const xhr = new XMLHttpRequest();
		xhr.open("GET", "https://api.github.com/repos/automatic9045/AtsEX/releases", true);
		xhr.onreadystatechange = (() => {
			if (xhr.readyState != 4) return;

			const resetTime = new Date(1000 * xhr.getResponseHeader("x-ratelimit-reset"));

			if (xhr.status == 200) {
				_result = {
					usesMirror: false,
					status: xhr.status,
					resetTime: resetTime,
					latestRelease: xhr.status == 200 ? JSON.parse(xhr.responseText)[0] : null
				};

				resolve(_result);
			} else {
				const xhr2 = new XMLHttpRequest();
				xhr2.open("GET", "https://script.google.com/macros/s/AKfycbzZXuIKyUOlBry5p3RF-S1Ephxyr9o-QKKnoC1R639pSqZZkvsxJoDUiQ_o4P1p5peK/exec", true);
				xhr2.onreadystatechange = (() => {
					if (xhr2.readyState != 4) return;

					const response = JSON.parse(xhr2.responseText);
					_result = {
						usesMirror: true,
						status: response.status,
						resetTime: resetTime,
						latestRelease: response.status == 200 ? response.content[0] : null
					};

					resolve(_result);
				});

				xhr2.send();
			}
		});
		xhr.send();
	} else {
		resolve(_result);
	}
});

let _thanksDialog = null;
function getThanksDialog() {
	if (_thanksDialog == null) {
		const element = document.getElementById("thanks");

		element.addEventListener("close", () => {
			document.documentElement.style.overflow = "inherit";
		});

		_thanksDialog = element;
	}

	return _thanksDialog;
}

function invokeTo(elementClassName, resolve) {
	const elements = document.getElementsByClassName(elementClassName);

	getLatestRelease.then(result => {
		Array.from(elements).forEach(element => {
			resolve(element, result);
        })
	});
}

function setInfoTo(elementClassName, resolve) {
	invokeTo(elementClassName, (element, result) => {
		if (result.status == 200) {
			element.innerHTML = resolve(result);
		} else {
			element.innerHTML = "<span class='light-text'>混雑等により情報を取得できませんでした (" + result.status + ")</span>";
		}
	});
}

function setLatestVersionTo(elementClassName) {
	setInfoTo(elementClassName, result => {
		return "<strong>" + result.latestRelease.name + "</strong> (" + new Date(result.latestRelease.created_at).toLocaleDateString() + ")";
    });
}

function setLatestReleaseNoteTo(elementClassName) {
	setInfoTo(elementClassName, result => {
		const markdown = result.latestRelease.body.replace(/\r\n#/g, "\r\n###");
		const html = marked.parse(markdown);

		return html;
	});
}

function setResetTimeTo(elementClassName) {
	invokeTo(elementClassName, (element, result) => {
		element.innerHTML = result.resetTime.toLocaleTimeString();
	});
}

function setPackageInfoTo(elementClassName, typeString, extensionString) {
	setInfoTo(elementClassName, result => {
		const asset = result.latestRelease.assets.find(x => x.name.endsWith("_" + typeString + "." + extensionString));
		return "<strong>" + asset.name + "</strong> (" + new Date(asset.created_at).toLocaleDateString() + ")";
	});
}

function setAsAtsPluginPackageInfoTo(elementClassName) {
	setPackageInfoTo(elementClassName, "AsAtsPlugin", "7z");
}

function setAsInputDevicePackageInfoTo(elementClassName) {
	setPackageInfoTo(elementClassName, "AsInputDevice", "7z");
}

function setAsInputDeviceInstallerPackageInfoTo(elementClassName) {
	setPackageInfoTo(elementClassName, "AsInputDevice.Setup", "exe");
}

function setSdkInfoTo(elementClassName) {
	setPackageInfoTo(elementClassName, "SDK", "7z");
}

function setPackageLinkTo(elementClassName, typeString, extensionString) {
	invokeTo(elementClassName, (element, result) => {
		const asset = result.latestRelease.assets.find(x => x.name.endsWith("_" + typeString + "." + extensionString));

		element.classList.remove("disabled");
		element.setAttribute("tabindex", 0);

		element.addEventListener("click", () => {
			const downloadElement = document.createElement("a");
			downloadElement.href = asset.browser_download_url;
			downloadElement.click();

			const manualDownloadElement = document.getElementsByClassName("release-manual-download")[0];
			manualDownloadElement.href = asset.browser_download_url;

			const dialog = getThanksDialog();
			dialog.showModal();
			dialog.scrollTop = 0;

			document.documentElement.style.overflow = "hidden";
		});
	});
}

function setAsAtsPluginPackageLinkTo(elementClassName) {
	setPackageLinkTo(elementClassName, "AsAtsPlugin", "7z");
}

function setAsInputDevicePackageLinkTo(elementClassName) {
	setPackageLinkTo(elementClassName, "AsInputDevice", "7z");
}

function setAsInputDeviceInstallerPackageLinkTo(elementClassName) {
	setPackageLinkTo(elementClassName, "AsInputDevice.Setup", "exe");
}

function setSdkLinkTo(elementClassName) {
	setPackageLinkTo(elementClassName, "SDK");
}

function hideIfFailedToGetRelease(elementClassName) {
	invokeTo(elementClassName, (element, result) => {
		if (result.status != 200) {
			element.style.display = "none";
		}
	});
}

function displayIfFailedToGetRelease(elementClassName) {
	invokeTo(elementClassName, (element, result) => {
		if (result.status != 200) {
			element.style.display = "block";
		}
	});
}

function displayIfUseMirrorToGetRelease(elementClassName) {
	invokeTo(elementClassName, (element, result) => {
		if (result.usesMirror) {
			element.style.display = "block";
		}
	});
}