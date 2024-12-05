const CACHE_NAME = 'lms-cache-v1';
const API_CACHE_NAME = 'lms-api-cache-v1';

const STATIC_ASSETS = [
    '/',
    '/css/app.css',
    '/js/app.js',
    '/images/logo.png',
    '/offline.html'
];

const API_ROUTES = [
    '/api/discussions',
    '/api/progress',
    '/api/certificates'
];

self.addEventListener('install', (event) => {
    event.waitUntil(
        Promise.all([
            caches.open(CACHE_NAME).then((cache) => {
                return cache.addAll(STATIC_ASSETS);
            }),
            caches.open(API_CACHE_NAME)
        ])
    );
});