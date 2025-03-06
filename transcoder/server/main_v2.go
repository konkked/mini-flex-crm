// package main

// import (
// 	"context"
// 	"fmt"
// 	"io"
// 	"log"
// 	"net/http"
// 	"os"
// 	"os/exec"
// 	"path/filepath"
// 	"sync"

// 	"github.com/aws/aws-sdk-go/aws/session"
// 	"github.com/aws/aws-sdk-go/service/s3/s3manager"
// )

// func main_v2() {
// 	http.HandleFunc("/upload", func(w http.ResponseWriter, r *http.Request) {
// 		if r.Method != http.MethodPost {
// 			http.Error(w, "Invalid request method", http.StatusMethodNotAllowed)
// 			return
// 		}

// 		streamID := r.FormValue("streamID")
// 		if streamID == "" {
// 			http.Error(w, "Missing streamID", http.StatusBadRequest)
// 			return
// 		}

// 		file, _, err := r.FormFile("file")
// 		if err != nil {
// 			http.Error(w, "Failed to get file from request", http.StatusBadRequest)
// 			return
// 		}
// 		defer file.Close()

// 		inputPath := filepath.Join(os.TempDir(), "input.ts")
// 		out, err := os.Create(inputPath)
// 		if err != nil {
// 			http.Error(w, "Failed to create input file", http.StatusInternalServerError)
// 			return
// 		}
// 		defer out.Close()

// 		if _, err := io.Copy(out, file); err != nil {
// 			http.Error(w, "Failed to save input file", http.StatusInternalServerError)
// 			return
// 		}

// 		t := NewTranscoder("my-stream-bucket")
// 		ctx := context.Background()
// 		if err := t.ProcessStream(ctx, streamID, inputPath); err != nil {
// 			http.Error(w, "Failed to process stream", http.StatusInternalServerError)
// 			return
// 		}

// 		w.WriteHeader(http.StatusOK)
// 		fmt.Fprintln(w, "Stream processed and uploaded successfully")
// 	})

// 	log.Fatal(http.ListenAndServe(":8080", nil))
// }
