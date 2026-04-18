export const defaultThresholds = {
  http_req_failed: ["rate<0.01"],
  http_req_duration: ["p(95)<800"],
};

export const moderateLoadThresholds = {
  http_req_failed: ["rate<0.02"],
  http_req_duration: ["p(95)<1200"],
};
