// package main

// import (
// 	"context"
// 	"fmt"
// 	"log"
// 	"os"
// 	"os/exec"
// 	"path/filepath"
// 	"strconv"
// 	"sync"
// 	"time"

// 	"github.com/aws/aws-sdk-go/aws"
// 	"github.com/aws/aws-sdk-go/aws/session"
// 	"github.com/aws/aws-sdk-go/service/s3/s3manager"
// 	"github.com/aws/aws-sdk-go/service/sqs"
// )

// // Resolution represents a video resolution with encoding parameters
// type Resolution struct {
// 	Name    string
// 	Width   int
// 	Height  int
// 	Bitrate string
// }

// // StreamProcessor handles the transcoding of live streams
// type StreamProcessor struct {
// 	inputPath      string
// 	outputBasePath string
// 	streamID       string
// 	s3Uploader     *s3manager.Uploader
// 	metadataClient *MetadataClient
// 	bucketName     string
// }

// // NewStreamProcessor creates a new stream processor
// func NewStreamProcessor(streamID string, bucketName string) (*StreamProcessor, error) {
// 	// Create temporary directories for processing
// 	inputPath := filepath.Join(os.TempDir(), "stream_input", streamID)
// 	outputBasePath := filepath.Join(os.TempDir(), "stream_output", streamID)

// 	// Ensure directories exist
// 	if err := os.MkdirAll(inputPath, 0755); err != nil {
// 		return nil, fmt.Errorf("failed to create input directory: %w", err)
// 	}
// 	if err := os.MkdirAll(outputBasePath, 0755); err != nil {
// 		return nil, fmt.Errorf("failed to create output directory: %w", err)
// 	}

// 	// Initialize AWS S3 session for uploads
// 	sess := session.Must(session.NewSession(&aws.Config{
// 		Region: aws.String("us-west-2"), // Default region, would be dynamic in production
// 	}))
// 	uploader := s3manager.NewUploader(sess)

// 	// Initialize metadata client
// 	metadataClient := NewMetadataClient()

// 	return &StreamProcessor{
// 		inputPath:      inputPath,
// 		outputBasePath: outputBasePath,
// 		streamID:       streamID,
// 		s3Uploader:     uploader,
// 		metadataClient: metadataClient,
// 		bucketName:     bucketName,
// 	}, nil
// }

// // ProcessStream handles the end-to-end processing of a stream
// func (p *StreamProcessor) ProcessStream(ctx context.Context) error {
// 	// Define resolutions to transcode to
// 	resolutions := []Resolution{
// 		{Name: "1080p", Width: 1920, Height: 1080, Bitrate: "5000k"},
// 		{Name: "720p", Width: 1280, Height: 720, Bitrate: "2500k"},
// 		{Name: "480p", Width: 854, Height: 480, Bitrate: "1000k"},
// 		{Name: "360p", Width: 640, Height: 360, Bitrate: "500k"},
// 	}

// 	// Update metadata to indicate processing has started
// 	if err := p.metadataClient.UpdateStreamStatus(p.streamID, "processing"); err != nil {
// 		return fmt.Errorf("failed to update stream status: %w", err)
// 	}

// 	// Process each resolution in parallel
// 	var wg sync.WaitGroup
// 	errCh := make(chan error, len(resolutions))

// 	for _, resolution := range resolutions {
// 		wg.Add(1)
// 		go func(res Resolution) {
// 			defer wg.Done()
// 			if err := p.processResolution(ctx, res); err != nil {
// 				errCh <- fmt.Errorf("failed to process %s: %w", res.Name, err)
// 			}
// 		}(resolution)
// 	}

// 	// Wait for all transcoding to complete
// 	wg.Wait()
// 	close(errCh)

// 	// Check for errors
// 	for err := range errCh {
// 		return err // Return the first error encountered
// 	}

// 	// Update metadata with available resolutions
// 	resolutionNames := make([]string, len(resolutions))
// 	for i, res := range resolutions {
// 		resolutionNames[i] = res.Name
// 	}

// 	if err := p.metadataClient.UpdateAvailableResolutions(p.streamID, resolutionNames); err != nil {
// 		return fmt.Errorf("failed to update available resolutions: %w", err)
// 	}

// 	return nil
// }

// // processResolution handles transcoding for a specific resolution
// func (p *StreamProcessor) processResolution(ctx context.Context, resolution Resolution) error {
// 	outputDir := filepath.Join(p.outputBasePath, resolution.Name)
// 	if err := os.MkdirAll(outputDir, 0755); err != nil {
// 		return fmt.Errorf("failed to create output directory for %s: %w", resolution.Name, err)
// 	}

// 	// Generate the HLS playlist and segments using precise 3.14 second chunks
// 	outputPath := filepath.Join(outputDir, "playlist.m3u8")
// 	segmentTemplate := filepath.Join(outputDir, "segment_%03d.ts")

// 	// Configure ffmpeg command
// 	args := []string{
// 		"-i", p.inputPath + "/input.ts",
// 		"-c:v", "libx264",
// 		"-preset", "veryfast",
// 		"-tune", "zerolatency",
// 		"-b:v", resolution.Bitrate,
// 		"-s", fmt.Sprintf("%dx%d", resolution.Width, resolution.Height),
// 		"-c:a", "aac",
// 		"-b:a", "128k",
// 		"-f", "hls",
// 		"-hls_time", "3.14", // Exactly 3.14 second chunks as requested
// 		"-hls_list_size", "0", // Keep all segments in the playlist
// 		"-hls_segment_filename", segmentTemplate,
// 		"-hls_flags", "independent_segments",
// 		outputPath,
// 	}

// 	cmd := exec.CommandContext(ctx, "ffmpeg", args...)
// 	cmd.Stdout = os.Stdout
// 	cmd.Stderr = os.Stderr

// 	if err := cmd.Run(); err != nil {
// 		return fmt.Errorf("ffmpeg command failed: %w", err)
// 	}

// 	// Upload segments and playlist to S3
// 	return p.uploadOutput(ctx, outputDir, resolution.Name)
// }

// // uploadOutput uploads the generated HLS segments and playlist to S3
// func (p *StreamProcessor) uploadOutput(ctx context.Context, outputDir string, resolutionName string) error {
// 	// Upload the playlist file
// 	playlistPath := filepath.Join(outputDir, "playlist.m3u8")
// 	s3KeyPrefix := fmt.Sprintf("%s/%s", p.streamID, resolutionName)

// 	playlistFile, err := os.Open(playlistPath)
// 	if err != nil {
// 		return fmt.Errorf("failed to open playlist file: %w", err)
// 	}
// 	defer playlistFile.Close()

// 	playlistKey := fmt.Sprintf("%s/playlist.m3u8", s3KeyPrefix)
// 	_, err = p.s3Uploader.UploadWithContext(ctx, &s3manager.UploadInput{
// 		Bucket: aws.String(p.bucketName),
// 		Key:    aws.String(playlistKey),
// 		Body:   playlistFile,
// 	})
// 	if err != nil {
// 		return fmt.Errorf("failed to upload playlist: %w", err)
// 	}

// 	// Upload all segment files
// 	files, err := os.ReadDir(outputDir)
// 	if err != nil {
// 		return fmt.Errorf("failed to read output directory: %w", err)
// 	}

// 	var wg sync.WaitGroup
// 	uploadErrCh := make(chan error, len(files))

// 	for _, file := range files {
// 		if filepath.Ext(file.Name()) == ".ts" {
// 			wg.Add(1)
// 			go func(fileName string) {
// 				defer wg.Done()

// 				filePath := filepath.Join(outputDir, fileName)
// 				fileHandle, err := os.Open(filePath)
// 				if err != nil {
// 					uploadErrCh <- fmt.Errorf("failed to open file %s: %w", fileName, err)
// 					return
// 				}
// 				defer fileHandle.Close()

// 				s3Key := fmt.Sprintf("%s/%s", s3KeyPrefix, fileName)
// 				_, err = p.s3Uploader.UploadWithContext(ctx, &s3manager.UploadInput{
// 					Bucket: aws.String(p.bucketName),
// 					Key:    aws.String(s3Key),
// 					Body:   fileHandle,
// 				})
// 				if err != nil {
// 					uploadErrCh <- fmt.Errorf("failed to upload segment %s: %w", fileName, err)
// 				}

// 				// Parse chunk number from filename for metadata updates
// 				chunkNumStr := fileName[8:11] // Extract "123" from "segment_123.ts"
// 				chunkNum, err := strconv.Atoi(chunkNumStr)
// 				if err != nil {
// 					uploadErrCh <- fmt.Errorf("failed to parse chunk number: %w", err)
// 					return
// 				}

// 				// Update metadata with chunk info (simplified, would be batched in production)
// 				if err := p.metadataClient.UpdateChunkInfo(p.streamID, chunkNum); err != nil {
// 					uploadErrCh <- fmt.Errorf("failed to update chunk info: %w", err)
// 				}
// 			}(file.Name())
// 		}
// 	}

// 	wg.Wait()
// 	close(uploadErrCh)

// 	// Check for upload errors
// 	for err := range uploadErrCh {
// 		return err // Return the first error encountered
// 	}

// 	return nil
// }

// // MetadataClient provides methods to interact with the metadata service
// type MetadataClient struct {
// 	// In a real implementation, this would have HTTP client, connection details, etc.
// }

// // NewMetadataClient creates a new metadata client
// func NewMetadataClient() *MetadataClient {
// 	return &MetadataClient{}
// }

// // UpdateStreamStatus updates the status of a stream
// func (c *MetadataClient) UpdateStreamStatus(streamID string, status string) error {
// 	// In a real implementation, this would make an API call to the metadata service
// 	log.Printf("Updating stream %s status to %s", streamID, status)
// 	return nil
// }

// // UpdateChunkInfo updates information about available chunks
// func (c *MetadataClient) UpdateChunkInfo(streamID string, chunkNumber int) error {
// 	// In a real implementation, this would make an API call to the metadata service
// 	log.Printf("Updating stream %s with chunk %d", streamID, chunkNumber)
// 	return nil
// }

// // UpdateAvailableResolutions updates the list of available resolutions for a stream
// func (c *MetadataClient) UpdateAvailableResolutions(streamID string, resolutions []string) error {
// 	// In a real implementation, this would make an API call to the metadata service
// 	log.Printf("Updating stream %s with resolutions: %v", streamID, resolutions)
// 	return nil
// }

// // processMessageQueue listens for incoming stream requests and processes them
// func processMessageQueue() {
// 	// Connect to message queue
// 	sess := session.Must(session.NewSession(&aws.Config{
// 		Region: aws.String("us-west-2"),
// 	}))

// 	svc := sqs.New(sess)

// 	queueURL := "https://sqs.us-west-2.amazonaws.com/123456789012/stream_processing"

// 	for {
// 		result, err := svc.ReceiveMessage(&sqs.ReceiveMessageInput{
// 			QueueUrl:            aws.String(queueURL),
// 			MaxNumberOfMessages: aws.Int64(1),
// 			WaitTimeSeconds:     aws.Int64(20),
// 		})
// 		if err != nil {
// 			log.Printf("Failed to receive message from SQS: %v", err)
// 			continue
// 		}

// 		if len(result.Messages) == 0 {
// 			continue
// 		}

// 		for _, message := range result.Messages {
// 			streamID := *message.Body
// 			log.Printf("Received stream processing request for: %s", streamID)

// 			processor, err := NewStreamProcessor(streamID, "streaming-platform-media")
// 			if err != nil {
// 				log.Printf("Failed to create stream processor: %v", err)
// 				continue
// 			}

// 			ctx, cancel := context.WithTimeout(context.Background(), 30*time.Minute)
// 			err = processor.ProcessStream(ctx)
// 			cancel()

// 			if err != nil {
// 				log.Printf("Failed to process stream: %v", err)
// 			} else {
// 				log.Printf("Successfully processed stream: %s", streamID)
// 			}

// 			_, err = svc.DeleteMessage(&sqs.DeleteMessageInput{
// 				QueueUrl:      aws.String(queueURL),
// 				ReceiptHandle: message.ReceiptHandle,
// 			})
// 			if err != nil {
// 				log.Printf("Failed to delete message from SQS: %v", err)
// 			}
// 		}
// 	}
// }

// func main() {
// 	processMessageQueue()
// }
