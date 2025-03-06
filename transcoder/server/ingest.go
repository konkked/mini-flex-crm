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

// 	"github.com/aws/aws-sdk-go/aws/session"
// 	"github.com/aws/aws-sdk-go/service/s3/s3manager"
// 	"github.com/gin-gonic/gin"

// 	"github.com/aws/aws-sdk-go/aws"
// 	"github.com/aws/aws-sdk-go/service/dynamodb"
// 	"github.com/aws/aws-sdk-go/service/dynamodb/dynamodbattribute"
// )

// type Config struct {
// 	IngestPort     int
// 	TranscoderAddr string
// 	S3Bucket       string
// 	AWSRegion      string
// 	WorkerCount    int
// }

// type ConfigProvider interface {
// 	GetConfig() Config
// }

// type EnvConfigProvider struct{}

// func (e *EnvConfigProvider) GetConfig() Config {
// 	cfg := Config{
// 		IngestPort:     1935, // Default
// 		TranscoderAddr: "localhost:8080",
// 		S3Bucket:       "nba_videos",
// 		AWSRegion:      "us-east-1",
// 		WorkerCount:    4, // Default number of workers
// 	}

// 	if portStr := os.Getenv("INGEST_PORT"); portStr != "" {
// 		if port, err := strconv.Atoi(portStr); err == nil {
// 			cfg.IngestPort = port
// 		}
// 	}

// 	if addr := os.Getenv("TRANSCODER_ADDR"); addr != "" {
// 		cfg.TranscoderAddr = addr
// 	}

// 	if bucket := os.Getenv("S3_BUCKET"); bucket != "" {
// 		cfg.S3Bucket = bucket
// 	}

// 	if region := os.Getenv("AWS_REGION"); region != "" {
// 		cfg.AWSRegion = region
// 	}

// 	if workerCountStr := os.Getenv("WORKER_COUNT"); workerCountStr != "" {
// 		if workerCount, err := strconv.Atoi(workerCountStr); err == nil {
// 			cfg.WorkerCount = workerCount
// 		}
// 	}

// 	return cfg
// }

// type StreamResolution struct {
// 	Name    string
// 	Width   int
// 	Height  int
// 	Bitrate string
// }

// type Transcoder struct {
// 	s3Uploader *s3manager.Uploader
// 	bucket     string
// }

// func NewTranscoder(bucket string) *Transcoder {
// 	sess := session.Must(session.NewSession())
// 	return &Transcoder{
// 		s3Uploader: s3manager.NewUploader(sess),
// 		bucket:     bucket,
// 	}
// }

// func (t *Transcoder) ProcessStream(ctx context.Context, streamID, inputPath string) error {
// 	resolutions := []StreamResolution{
// 		{"1080p", 1920, 1080, "5000k"},
// 		{"720p", 1280, 720, "2500k"},
// 		{"480p", 854, 480, "1000k"},
// 		{"360p", 640, 360, "500k"},
// 	}

// 	outputDir := filepath.Join(os.TempDir(), streamID)
// 	if err := os.MkdirAll(outputDir, 0755); err != nil {
// 		return err
// 	}

// 	var wg sync.WaitGroup
// 	errCh := make(chan error, len(resolutions))

// 	for _, res := range resolutions {
// 		wg.Add(1)
// 		go func(r StreamResolution) {
// 			defer wg.Done()
// 			resDir := filepath.Join(outputDir, r.Name)
// 			if err := os.MkdirAll(resDir, 0755); err != nil {
// 				errCh <- err
// 				return
// 			}

// 			cmd := exec.CommandContext(ctx, "ffmpeg",
// 				"-i", inputPath,
// 				"-c:v", "libx264", "-preset", "veryfast", "-tune", "zerolatency",
// 				"-b:v", r.Bitrate, "-s", fmt.Sprintf("%dx%d", r.Width, r.Height),
// 				"-c:a", "aac", "-b:a", "128k",
// 				"-f", "hls", "-hls_time", "3.14", "-hls_list_size", "0",
// 				"-hls_segment_filename", filepath.Join(resDir, "chunk_%03d.ts"),
// 				filepath.Join(resDir, "playlist.m3u8"),
// 			)
// 			if err := cmd.Run(); err != nil {
// 				errCh <- err
// 				return
// 			}

// 			// Upload to S3
// 			err := filepath.Walk(resDir, func(path string, info os.FileInfo, err error) error {
// 				if err != nil || info.IsDir() {
// 					return err
// 				}
// 				f, err := os.Open(path)
// 				if err != nil {
// 					return err
// 				}
// 				defer f.Close()
// 				key := fmt.Sprintf("%s/%s/%s", streamID, r.Name, info.Name())
// 				_, err = t.s3Uploader.Upload(&s3manager.UploadInput{
// 					Bucket: &t.bucket,
// 					Key:    &key,
// 					Body:   f,
// 				})
// 				return err
// 			})

// 			if err != nil {
// 				errCh <- err
// 			}
// 		}(res)
// 	}

// 	wg.Wait()
// 	close(errCh)
// 	for err := range errCh {
// 		return err
// 	}
// 	return nil
// }

// type IngestServer struct {
// 	configProvider ConfigProvider
// 	transcoder     *Transcoder
// 	taskQueue      chan TranscodeTask
// }

// type TranscodeTask struct {
// 	StreamID   string
// 	InputPath  string
// 	Context    context.Context
// 	Completion chan error
// }

// type StreamRecord struct {
// 	Id                  string `json:"id"`
// 	Live                bool   `json:"live"`
// 	BroadcasterRecordId string `json:"broadcaster_record_id"`
// }

// type BroadcasterRecord struct {
// 	Id        string `json:"id"`
// 	StreamKey string `json:"stream_key"`
// 	Enabled   bool   `json:"enabled"`
// }

// func (s *IngestServer) isValidStreamId(streamID string) bool {
// 	sess := session.Must(session.NewSession(&aws.Config{
// 		Region: aws.String(s.configProvider.GetConfig().AWSRegion),
// 	}))
// 	svc := dynamodb.New(sess)

// 	result, err := svc.GetItem(&dynamodb.GetItemInput{
// 		TableName: aws.String("event_streams"),
// 		Key: map[string]*dynamodb.AttributeValue{
// 			"stream_id": {
// 				S: aws.String(streamID),
// 			},
// 		},
// 	})
// 	if err != nil {
// 		log.Printf("failed to get item from DynamoDB: %v", err)
// 		return false
// 	}

// 	if result.Item == nil {
// 		return false
// 	}

// 	var streamKeyItem StreamRecord
// 	if err := dynamodbattribute.UnmarshalMap(result.Item, &streamKeyItem); err != nil {
// 		log.Printf("failed to unmarshal DynamoDB item: %v", err)
// 		return false
// 	}

// 	return streamKeyItem.Live
// }

// func (s *IngestServer) isRequestValid(streamId string, streamKey string, streamBroadcasterId string) bool {
// 	sess := session.Must(session.NewSession(&aws.Config{
// 		Region: aws.String(s.configProvider.GetConfig().AWSRegion),
// 	}))
// 	svc := dynamodb.New(sess)

// 	streamRecordResult, err := svc.GetItem(&dynamodb.GetItemInput{
// 		TableName: aws.String("streams"),
// 		Key: map[string]*dynamodb.AttributeValue{
// 			"id": {
// 				S: aws.String(streamId),
// 			},
// 		},
// 	})

// 	if err != nil {
// 		log.Printf("failed to get item from DynamoDB: %v", err)
// 		return false
// 	}

// 	if streamRecordResult.Item == nil {
// 		return false
// 	}

// 	var streamRecord StreamRecord
// 	if err := dynamodbattribute.UnmarshalMap(streamRecordResult.Item, &streamRecord); err != nil {
// 		log.Printf("failed to unmarshal DynamoDB item: %v", err)
// 		return false
// 	}

// 	if !streamRecord.Live || streamRecord.BroadcasterRecordId != streamBroadcasterId {
// 		return false
// 	}

// 	broadcasterRecordResult, err := svc.GetItem(&dynamodb.GetItemInput{
// 		TableName: aws.String("broadcasters"),
// 		Key: map[string]*dynamodb.AttributeValue{
// 			"id": {
// 				S: aws.String(streamBroadcasterId),
// 			},
// 		},
// 	})

// 	if err != nil {
// 		log.Printf("failed to get item from DynamoDB: %v", err)
// 		return false
// 	}

// 	if broadcasterRecordResult.Item == nil {
// 		return false
// 	}

// 	if broadcasterRecordResult.Item["stream_key"].S == nil {
// 		log.Printf("stream key is empty")
// 		return false
// 	}

// 	if *broadcasterRecordResult.Item["stream_key"].S != streamKey {
// 		log.Printf("stream key does not match")
// 		return false
// 	}

// 	if broadcasterRecordResult.Item["enabled"].BOOL == nil {
// 		log.Printf("enabled is empty")
// 		return false
// 	}

// 	if !*broadcasterRecordResult.Item["enabled"].BOOL {
// 		log.Printf("broadcaster is not enabled")
// 		return false
// 	}

// 	return true
// }

// func NewIngestServer(provider ConfigProvider) *IngestServer {
// 	cfg := provider.GetConfig()
// 	server := &IngestServer{
// 		configProvider: provider,
// 		transcoder:     NewTranscoder(cfg.S3Bucket),
// 		taskQueue:      make(chan TranscodeTask, cfg.WorkerCount),
// 	}

// 	for i := 0; i < cfg.WorkerCount; i++ {
// 		go server.worker()
// 	}

// 	return server
// }

// func (s *IngestServer) worker() {
// 	for task := range s.taskQueue {
// 		err := s.transcoder.ProcessStream(task.Context, task.StreamID, task.InputPath)
// 		task.Completion <- err
// 	}
// }

// func (s *IngestServer) Start() error {
// 	cfg := s.configProvider.GetConfig()
// 	router := gin.Default()

// 	router.POST("broadcaster/:broadcaster_id/stream/:stream_id/upload", func(c *gin.Context) {
// 		streamID := c.Param("stream_id")
// 		broadcasterID := c.Param("broadcaster_id")
// 		streamKey := c.Request.Header.Get("X-Stream-Key")

// 		if streamID == "" || streamKey == "" || broadcasterID == "" {
// 			log.Printf("missing stream ID, stream key, or broadcaster ID")
// 			c.String(400, "missing stream ID, stream key, or broadcaster ID")
// 			return
// 		}

// 		if !s.isRequestValid(streamID, streamKey, "broadcaster_record_id") {
// 			log.Printf("invalid request for %s", streamID)
// 			c.String(403, "Invalid request")
// 			return
// 		}

// 		inputPath := filepath.Join(os.TempDir(), fmt.Sprintf("%s.flv", streamID))
// 		file, err := os.Create(inputPath)
// 		if err != nil {
// 			log.Printf("failed to create input file: %v", err)
// 			c.String(500, "Internal server error")
// 			return
// 		}

// 		defer file.Close()

// 		if _, err := file.ReadFrom(c.Request.Body); err != nil {
// 			log.Printf("failed to read request body: %v", err)
// 			c.String(500, "Internal server error")
// 			return
// 		}

// 		ctx := context.Background()
// 		completion := make(chan error)
// 		s.taskQueue <- TranscodeTask{
// 			StreamID:   streamID,
// 			InputPath:  inputPath,
// 			Context:    ctx,
// 			Completion: completion,
// 		}

// 		if err := <-completion; err != nil {
// 			log.Printf("transcoding failed: %v", err)
// 			c.String(500, "Internal server error")
// 			return
// 		}

// 		c.String(200, "Stream uploaded and processed successfully")
// 	})

// 	log.Printf("Ingest server started on :%d", cfg.IngestPort)
// 	return router.Run(fmt.Sprintf(":%d", cfg.IngestPort))
// }

// func ingest() {
// 	s := NewIngestServer(&EnvConfigProvider{})
// 	if err := s.Start(); err != nil {
// 		log.Fatal(err)
// 	}
// }
