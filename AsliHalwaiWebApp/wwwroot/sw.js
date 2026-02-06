const CACHE_NAME = "aslihalwai-static-v1";

self.addEventListener("install", (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => {
            return cache.addAll([
                "/",
                "/css/site.css",
                "/Template/assets/css/style.css",
                "/Template/assets/lib/bootstrap-4.5.0/css/bootstrap.css",
                "/icons/icon-192.png",
                "/icons/icon-512.png"
            ]);
        })
    );
    self.skipWaiting();
});

self.addEventListener("activate", (event) => {
    event.waitUntil(
        caches.keys().then((keys) =>
            Promise.all(
                keys.map((key) => {
                    if (key !== CACHE_NAME) {
                        return caches.delete(key);
                    }
                })
            )
        )
    );
    self.clients.claim();
});

self.addEventListener("fetch", (event) => {
    // ❌ DO NOT CACHE HTML / MVC ROUTES
    if (event.request.headers.get("accept")?.includes("text/html")) {
        return;
    }

    event.respondWith(
        caches.match(event.request).then((response) => {
            return response || fetch(event.request);
        })
    );
});
