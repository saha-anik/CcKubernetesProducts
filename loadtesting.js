import http from 'k6/http';
import { sleep } from 'k6';
export let options = {
  vus: 500,
  duration: '100s',
};
export default function () {
  http.get('http://20.52.212.203/');
  sleep(1);
}
