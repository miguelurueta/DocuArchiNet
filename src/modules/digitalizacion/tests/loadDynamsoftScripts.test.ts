import { beforeEach, describe, expect, it } from "vitest";
import {
  loadDynamsoftScripts,
  resetDynamsoftScriptLoaderForTests,
} from "../infrastructure/dynamsoft/loadDynamsoftScripts";
import { DYNAMSOFT_SCRIPT_ID } from "../infrastructure/dynamsoft";

describe("[SPEC:SCRUMCORE-240] loadDynamsoftScripts", () => {
  beforeEach(() => {
    resetDynamsoftScriptLoaderForTests();
    document.head.innerHTML = "";
  });

  it("loads script once and reuses the same promise", async () => {
    const first = loadDynamsoftScripts({ scriptSrc: "/dwt.js" });
    const second = loadDynamsoftScripts({ scriptSrc: "/dwt.js" });
    const script = document.getElementById(DYNAMSOFT_SCRIPT_ID);

    expect(first).toBe(second);
    expect(script).toBeInstanceOf(HTMLScriptElement);

    script?.dispatchEvent(new Event("load"));

    await expect(first).resolves.toBeUndefined();
    expect(document.querySelectorAll(`#${DYNAMSOFT_SCRIPT_ID}`)).toHaveLength(1);
  });

  it("returns controlled error when script fails", async () => {
    const loadPromise = loadDynamsoftScripts({ scriptSrc: "/broken.js" });
    const script = document.getElementById(DYNAMSOFT_SCRIPT_ID);

    script?.dispatchEvent(new Event("error"));

    await expect(loadPromise).rejects.toMatchObject({
      code: "DYNAMSOFT_SCRIPT_LOAD_FAILED",
    });
  });
});
