export const debugDynamsoftLicense = (stage: string, licenseKey?: string) => {
  const value = licenseKey ?? "";

  console.debug("[DynamsoftLicense]", stage, {
    length: value.length,
    first5: value.slice(0, 5),
    last5: value.slice(-5),
  });
};
