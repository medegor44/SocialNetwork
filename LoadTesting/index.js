import http from "k6/http";
import exec from "k6/execution";
import { russianName } from "russian_name/lib";
import qs from "querystringify";
import config from "./config.local.json";

export const options = config;

export default function () {
  console.log(options);
  const name = russianName.one(Math.random() > 0.5 ? "male" : "female");
  const host = options.myHost;
  const method = "User/get";

  const params = qs.stringify({
    firstName: name.name,
    secondName: name.surname,
    limit: Math.floor(200 * Math.random()),
  });

  const url = `${host}/${method}?${params}`;
  console.log(url);
  // http.get(url);
}
