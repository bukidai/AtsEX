const year = new Date().getFullYear();
const content = "&copy; " + (year == 2022 ? "2022" : "2022-" + year) + " automatic9045";
setHTMLByClasses(content, "footer-basic-copyright");