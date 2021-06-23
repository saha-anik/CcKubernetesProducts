import http from 'k6/http';
import { sleep } from 'k6';
export let options = {
  vus: 200,
  duration: '60s',
};
export default function () {
  http.get('http://20.79.117.111/Products');
  sleep(1);
}
