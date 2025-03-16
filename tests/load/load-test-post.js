import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 20,
    duration: '20s',
};

export default function () {
    const url = 'http://localhost:8080/api/bids';

    // Generate dynamic test data
    const payload = JSON.stringify({
        AuctionId: Math.floor(Math.random() * 10).toString(),
        CarId: Math.floor(Math.random() * 10).toString(),
        BidderName: `User_${Math.floor(Math.random() * 1000)}`,
        Amount: Math.random() * 10000
    });

    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    const res = http.post(url, payload, params);

    check(res, {
        'is status 200': (r) => r.status === 200,
    });

    sleep(1);
}
