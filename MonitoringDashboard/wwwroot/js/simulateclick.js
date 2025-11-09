window.simulateClick = (x, y) => {
    const el = document.elementFromPoint(x, y);
    console.log("Simulating click at:", x, y, "on element:", el);
    if (el) el.click();
};