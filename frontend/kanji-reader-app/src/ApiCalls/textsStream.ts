import { useEffect, useRef } from "react";
import { type ProcessingResult } from "./texts";

const API_URL = "http://localhost:5000/api/texts/stream";

export function useTextsStream(onItem: (items: ProcessingResult[]) => void) {
  const onItemRef = useRef(onItem);
  onItemRef.current = onItem;

  useEffect(() => {
    let es: EventSource | null = null;
    let retry = 1000;

    function connect() {
      es = new EventSource(API_URL, { withCredentials: false });

      es.onmessage = (e) => {
        try {
          const data = JSON.parse(e.data);
          const items = Array.isArray(data) ? data : [data];
          onItemRef.current(items);
          console.log("Received text stream update:", items);
        } catch (error) {
          console.error("Error parsing text stream data:", error);
        }
      };

      es.addEventListener("newText", (e) => {
        try {
          const ev = e as MessageEvent;
          const data = JSON.parse(ev.data);
          const items = Array.isArray(data) ? data : [data];
          onItemRef.current(items);
          console.log("Received newText event:", items);
        } catch (error) {
          console.error("Error parsing newText event data:", error);
        }
      });

      es.onerror = () => {
        es?.close();
        setTimeout(connect, retry);
        retry = Math.min(retry * 2, 15000);
        console.log("error useTextsStream");
      };
    }

    connect();
    return () => es?.close();
  }, []);
}
