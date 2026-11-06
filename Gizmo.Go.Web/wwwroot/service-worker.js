// In development, always fetch from the network and do not enable offline support.
// Caching would hide changes on the first load after each build.
self.addEventListener('fetch', () => { });
