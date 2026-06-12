import { beforeEach, describe, expect, it } from "vitest";
import {
  buildDynamsoftCssUrls,
  loadDynamsoftScripts,
  resetDynamsoftScriptLoaderForTests,
} from "../infrastructure/dynamsoft/loadDynamsoftScripts";
import {
  DYNAMSOFT_CSS_ID_PREFIX,
  DYNAMSOFT_SCRIPT_ID,
} from "../infrastructure/dynamsoft";

describe("[SPEC:SCRUMCORE-240] loadDynamsoftScripts", () => {
  beforeEach(() => {
    resetDynamsoftScriptLoaderForTests();
    document.head.innerHTML = "";
  });

  it("loads script once and reuses the same promise", async () => {
    const first = loadDynamsoftScripts({ scriptSrc: "/dwt.js" });
    const second = loadDynamsoftScripts({ scriptSrc: "/dwt.js" });
    const script = document.getElementById(DYNAMSOFT_SCRIPT_ID);

    expect(script).toBeInstanceOf(HTMLScriptElement);

    script?.dispatchEvent(new Event("load"));
    document
      .querySelectorAll(`link[id^="${DYNAMSOFT_CSS_ID_PREFIX}"]`)
      .forEach((link) => link.dispatchEvent(new Event("load")));

    await expect(first).resolves.toBeUndefined();
    await expect(second).resolves.toBeUndefined();
    expect(document.querySelectorAll(`#${DYNAMSOFT_SCRIPT_ID}`)).toHaveLength(1);
    expect(
      document.querySelectorAll(`link[id^="${DYNAMSOFT_CSS_ID_PREFIX}"]`),
    ).toHaveLength(2);
  });

  it("returns controlled error when script fails", async () => {
    const loadPromise = loadDynamsoftScripts({ scriptSrc: "/broken.js" });
    const script = document.getElementById(DYNAMSOFT_SCRIPT_ID);

    script?.dispatchEvent(new Event("error"));

    await expect(loadPromise).rejects.toMatchObject({
      code: "DYNAMSOFT_SCRIPT_LOAD_FAILED",
    });
  });

  it("returns controlled error when css fails", async () => {
    const loadPromise = loadDynamsoftScripts({ scriptSrc: "/dwt.js" });
    const script = document.getElementById(DYNAMSOFT_SCRIPT_ID);

    script?.dispatchEvent(new Event("load"));
    document
      .querySelector(`link[id^="${DYNAMSOFT_CSS_ID_PREFIX}"]`)
      ?.dispatchEvent(new Event("error"));

    await expect(loadPromise).rejects.toMatchObject({
      code: "DYNAMSOFT_CSS_LOAD_FAILED",
    });
  });

  it("builds css urls from resources path", () => {
    expect(buildDynamsoftCssUrls("https://cdn.example.com/dwt/dist/")).toEqual([
      "https://cdn.example.com/dwt/dist/src/dynamsoft.webtwain.css",
      "https://cdn.example.com/dwt/dist/src/dynamsoft.webtwain.viewer.css",
    ]);
  });
});
