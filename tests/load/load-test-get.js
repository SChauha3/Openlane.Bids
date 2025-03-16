import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 50, // Virtual Users
    duration: '30s', // Duration of test
};

export default function () {
    const url = 'http://localhost:8080/api/bids?auctionId=1&carId=1&cursor=10&pageSize=10';

    const res = http.get(url);

    // Check response status
    check(res, {
        'is status 200': (r) => r.status === 200,
        'body is not empty': (r) => r.body.length > 0,
    });

    sleep(1); // Sleep for 1 second between requests
}