import { AppVisorEmbedPdf } from "../Components/UI/AppVisorEmbedPdf";

export default function PlaywrightEmbedPdfPage() {
  return (
    <div style={{ width: "100vw", height: "100vh" }}>
      <AppVisorEmbedPdf fileUrl="/demo/oficio-prueba.pdf" />
    </div>
  );
}

