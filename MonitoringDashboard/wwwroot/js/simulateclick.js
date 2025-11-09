window.simulateClick = (x, y) => {
    try {
        const el = document.elementFromPoint(x, y);
        if (el) el.click();
    } catch (e) {}
};