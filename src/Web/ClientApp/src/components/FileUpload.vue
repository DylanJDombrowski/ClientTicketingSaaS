<template>
  <div class="space-y-4">
    <!-- Upload Area -->
    <div
      ref="dropZone"
      @dragover.prevent="handleDragOver"
      @dragleave.prevent="handleDragLeave"
      @drop.prevent="handleDrop"
      @click="triggerFileInput"
      class="border-2 border-dashed rounded-lg p-8 text-center transition-all duration-200 cursor-pointer"
      :class="{
        'border-blue-400 bg-blue-50': isDragging,
        'border-gray-300 hover:border-gray-400': !isDragging && !uploading,
        'border-green-400 bg-green-50': uploading,
        'opacity-50 cursor-not-allowed': disabled,
      }"
    >
      <input
        ref="fileInput"
        type="file"
        :multiple="multiple"
        :accept="accept"
        @change="handleFileSelect"
        class="hidden"
        :disabled="disabled"
      />

      <!-- Upload States -->
      <div v-if="uploading" class="space-y-3">
        <div
          class="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"
        ></div>
        <div>
          <p class="text-sm font-medium text-gray-900">Uploading files...</p>
          <div class="w-full bg-gray-200 rounded-full h-2 mt-2">
            <div
              class="bg-blue-600 h-2 rounded-full transition-all duration-300"
              :style="{ width: uploadProgress + '%' }"
            ></div>
          </div>
          <p class="text-xs text-gray-500 mt-1">
            {{ uploadProgress }}% complete
          </p>
        </div>
      </div>

      <div v-else-if="isDragging" class="space-y-3">
        <ArrowDownTrayIcon class="h-12 w-12 text-blue-500 mx-auto" />
        <div>
          <p class="text-lg font-medium text-blue-900">Drop files here</p>
          <p class="text-sm text-blue-600">Release to upload your files</p>
        </div>
      </div>

      <div v-else class="space-y-3">
        <CloudArrowUpIcon class="h-12 w-12 text-gray-400 mx-auto" />
        <div>
          <p class="text-lg font-medium text-gray-900">
            {{ multiple ? "Upload files" : "Upload a file" }}
          </p>
          <p class="text-sm text-gray-500">
            Drag and drop {{ multiple ? "files" : "a file" }} here, or click to
            browse
          </p>
          <p v-if="accept" class="text-xs text-gray-400 mt-1">
            Supported formats: {{ acceptedFormats.join(", ").toUpperCase() }}
          </p>
          <p v-if="maxSize" class="text-xs text-gray-400">
            Maximum file size: {{ formatFileSize(maxSize) }}
          </p>
        </div>
      </div>
    </div>

    <!-- File List -->
    <div v-if="files.length > 0" class="space-y-2">
      <h4 class="text-sm font-medium text-gray-900">
        {{ multiple ? "Uploaded Files" : "Uploaded File" }} ({{ files.length }})
      </h4>

      <div class="space-y-2 max-h-60 overflow-y-auto">
        <div
          v-for="file in files"
          :key="file.id"
          class="flex items-center justify-between p-3 bg-gray-50 rounded-lg"
        >
          <div class="flex items-center space-x-3">
            <!-- File Icon -->
            <div class="flex-shrink-0">
              <DocumentIcon
                v-if="file.type.startsWith('text') || file.type.includes('pdf')"
                class="h-8 w-8 text-red-500"
              />
              <PhotoIcon
                v-else-if="file.type.startsWith('image')"
                class="h-8 w-8 text-green-500"
              />
              <FilmIcon
                v-else-if="file.type.startsWith('video')"
                class="h-8 w-8 text-blue-500"
              />
              <DocumentIcon v-else class="h-8 w-8 text-gray-500" />
            </div>

            <!-- File Info -->
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 truncate">
                {{ file.name }}
              </p>
              <div class="flex items-center space-x-2 mt-1">
                <p class="text-xs text-gray-500">
                  {{ formatFileSize(file.size) }}
                </p>
                <span
                  class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium"
                  :class="{
                    'bg-green-100 text-green-800': file.status === 'uploaded',
                    'bg-blue-100 text-blue-800': file.status === 'uploading',
                    'bg-red-100 text-red-800': file.status === 'error',
                  }"
                >
                  {{ file.status }}
                </span>
              </div>

              <!-- Upload Progress -->
              <div
                v-if="file.status === 'uploading'"
                class="w-full bg-gray-200 rounded-full h-1 mt-2"
              >
                <div
                  class="bg-blue-600 h-1 rounded-full transition-all duration-300"
                  :style="{ width: file.progress + '%' }"
                ></div>
              </div>

              <!-- Error Message -->
              <p
                v-if="file.status === 'error' && file.error"
                class="text-xs text-red-600 mt-1"
              >
                {{ file.error }}
              </p>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex items-center space-x-2">
            <!-- Download Button (for uploaded files) -->
            <button
              v-if="file.status === 'uploaded' && file.url"
              @click="downloadFile(file)"
              class="text-blue-600 hover:text-blue-800 p-1"
              title="Download file"
            >
              <ArrowDownTrayIcon class="h-4 w-4" />
            </button>

            <!-- Remove Button -->
            <button
              @click="removeFile(file.id)"
              class="text-red-600 hover:text-red-800 p-1"
              title="Remove file"
              :disabled="file.status === 'uploading'"
            >
              <XMarkIcon class="h-4 w-4" />
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Upload Summary -->
    <div v-if="files.length > 0" class="bg-blue-50 rounded-lg p-4">
      <div class="flex items-center justify-between">
        <div class="flex items-center space-x-2">
          <InformationCircleIcon class="h-5 w-5 text-blue-500" />
          <span class="text-sm text-blue-900">
            {{ uploadedCount }} of {{ files.length }} file{{
              files.length !== 1 ? "s" : ""
            }}
            uploaded ({{ formatFileSize(totalSize) }} total)
          </span>
        </div>

        <!-- Clear All Button -->
        <button
          v-if="files.length > 0 && !uploading"
          @click="clearAllFiles"
          class="text-sm text-red-600 hover:text-red-800 font-medium"
        >
          Clear All
        </button>
      </div>
    </div>

    <!-- Error Messages -->
    <div v-if="errors.length > 0" class="space-y-1">
      <div
        v-for="error in errors"
        :key="error.id"
        class="flex items-center justify-between bg-red-50 border border-red-200 rounded-lg p-3"
      >
        <div class="flex items-center space-x-2">
          <ExclamationCircleIcon class="h-4 w-4 text-red-500" />
          <span class="text-sm text-red-800">{{ error.message }}</span>
        </div>
        <button
          @click="dismissError(error.id)"
          class="text-red-600 hover:text-red-800"
        >
          <XMarkIcon class="h-4 w-4" />
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick } from "vue";
import {
  CloudArrowUpIcon,
  ArrowDownTrayIcon,
  DocumentIcon,
  PhotoIcon,
  FilmIcon,
  XMarkIcon,
  InformationCircleIcon,
  ExclamationCircleIcon,
} from "@heroicons/vue/24/outline";

interface FileUpload {
  id: string;
  name: string;
  size: number;
  type: string;
  status: "uploading" | "uploaded" | "error";
  progress: number;
  url?: string;
  error?: string;
  file: File;
}

interface UploadError {
  id: string;
  message: string;
}

interface Props {
  multiple?: boolean;
  accept?: string;
  maxSize?: number; // in bytes
  maxFiles?: number;
  disabled?: boolean;
  autoUpload?: boolean;
}

interface Emits {
  (e: "upload", files: FileUpload[]): void;
  (e: "remove", fileId: string): void;
  (e: "error", error: string): void;
}

const props = withDefaults(defineProps<Props>(), {
  multiple: true,
  accept: "*/*",
  maxSize: 10 * 1024 * 1024, // 10MB default
  maxFiles: 10,
  disabled: false,
  autoUpload: true,
});

const emit = defineEmits<Emits>();

// State
const isDragging = ref(false);
const uploading = ref(false);
const uploadProgress = ref(0);
const files = ref<FileUpload[]>([]);
const errors = ref<UploadError[]>([]);

// Refs
const dropZone = ref<HTMLElement>();
const fileInput = ref<HTMLInputElement>();

// Computed
const acceptedFormats = computed(() => {
  if (props.accept === "*/*") return ["All files"];
  return props.accept.split(",").map((type) => type.trim().replace(".", ""));
});

const uploadedCount = computed(() => {
  return files.value.filter((f) => f.status === "uploaded").length;
});

const totalSize = computed(() => {
  return files.value.reduce((total, file) => total + file.size, 0);
});

// Methods
const triggerFileInput = () => {
  if (props.disabled || uploading.value) return;
  fileInput.value?.click();
};

const handleDragOver = (e: DragEvent) => {
  if (props.disabled || uploading.value) return;
  isDragging.value = true;
};

const handleDragLeave = (e: DragEvent) => {
  // Check if we're actually leaving the drop zone
  if (!dropZone.value?.contains(e.relatedTarget as Node)) {
    isDragging.value = false;
  }
};

const handleDrop = (e: DragEvent) => {
  isDragging.value = false;
  if (props.disabled || uploading.value) return;

  const droppedFiles = Array.from(e.dataTransfer?.files || []);
  processFiles(droppedFiles);
};

const handleFileSelect = (e: Event) => {
  const target = e.target as HTMLInputElement;
  const selectedFiles = Array.from(target.files || []);
  processFiles(selectedFiles);

  // Reset input
  target.value = "";
};

const processFiles = (newFiles: File[]) => {
  // Clear previous errors
  errors.value = [];

  // Check max files limit
  if (files.value.length + newFiles.length > props.maxFiles) {
    addError(`Maximum ${props.maxFiles} files allowed`);
    return;
  }

  const validFiles: File[] = [];

  // Validate each file
  newFiles.forEach((file) => {
    // Check file size
    if (file.size > props.maxSize) {
      addError(
        `File "${file.name}" is too large. Maximum size: ${formatFileSize(
          props.maxSize
        )}`
      );
      return;
    }

    // Check file type
    if (props.accept !== "*/*" && !isFileTypeAccepted(file)) {
      addError(`File type "${file.type}" is not supported`);
      return;
    }

    // Check for duplicates
    if (files.value.some((f) => f.name === file.name && f.size === file.size)) {
      addError(`File "${file.name}" is already uploaded`);
      return;
    }

    validFiles.push(file);
  });

  // Add valid files
  validFiles.forEach((file) => {
    const fileUpload: FileUpload = {
      id: Math.random().toString(36).substr(2, 9),
      name: file.name,
      size: file.size,
      type: file.type,
      status: "uploading",
      progress: 0,
      file,
    };

    files.value.push(fileUpload);
  });

  // Auto upload if enabled
  if (props.autoUpload && validFiles.length > 0) {
    uploadFiles();
  }

  // Emit files
  emit("upload", files.value);
};

const isFileTypeAccepted = (file: File): boolean => {
  if (props.accept === "*/*") return true;

  const acceptedTypes = props.accept.split(",").map((type) => type.trim());

  return acceptedTypes.some((acceptedType) => {
    if (acceptedType.startsWith(".")) {
      // File extension check
      return file.name.toLowerCase().endsWith(acceptedType.toLowerCase());
    } else if (acceptedType.includes("*")) {
      // MIME type wildcard check
      const pattern = acceptedType.replace("*", ".*");
      return new RegExp(pattern).test(file.type);
    } else {
      // Exact MIME type check
      return file.type === acceptedType;
    }
  });
};

const uploadFiles = async () => {
  uploading.value = true;
  uploadProgress.value = 0;

  const pendingFiles = files.value.filter((f) => f.status === "uploading");

  try {
    // Simulate file uploads
    for (let i = 0; i < pendingFiles.length; i++) {
      const file = pendingFiles[i];
      await uploadSingleFile(file);
      uploadProgress.value = Math.round(((i + 1) / pendingFiles.length) * 100);
    }
  } catch (error) {
    console.error("Upload error:", error);
  } finally {
    uploading.value = false;
    uploadProgress.value = 0;
  }
};

const uploadSingleFile = async (fileUpload: FileUpload): Promise<void> => {
  try {
    // Simulate upload progress
    for (let progress = 0; progress <= 100; progress += 10) {
      fileUpload.progress = progress;
      await new Promise((resolve) => setTimeout(resolve, 100));
    }

    // Mock successful upload
    fileUpload.status = "uploaded";
    fileUpload.url = URL.createObjectURL(fileUpload.file); // Mock URL
    fileUpload.progress = 100;

    // Randomly simulate some upload failures for demo
    if (Math.random() < 0.1) {
      // 10% chance of failure
      throw new Error("Network error occurred");
    }
  } catch (error) {
    fileUpload.status = "error";
    fileUpload.error = error instanceof Error ? error.message : "Upload failed";
    fileUpload.progress = 0;
  }
};

const removeFile = (fileId: string) => {
  const fileIndex = files.value.findIndex((f) => f.id === fileId);
  if (fileIndex !== -1) {
    const file = files.value[fileIndex];

    // Revoke object URL to prevent memory leaks
    if (file.url) {
      URL.revokeObjectURL(file.url);
    }

    files.value.splice(fileIndex, 1);
    emit("remove", fileId);
  }
};

const downloadFile = (file: FileUpload) => {
  if (file.url) {
    const a = document.createElement("a");
    a.href = file.url;
    a.download = file.name;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
  }
};

const clearAllFiles = () => {
  // Revoke all object URLs
  files.value.forEach((file) => {
    if (file.url) {
      URL.revokeObjectURL(file.url);
    }
  });

  files.value = [];
  errors.value = [];
};

const addError = (message: string) => {
  const error: UploadError = {
    id: Math.random().toString(36).substr(2, 9),
    message,
  };

  errors.value.push(error);
  emit("error", message);

  // Auto-dismiss error after 5 seconds
  setTimeout(() => {
    dismissError(error.id);
  }, 5000);
};

const dismissError = (errorId: string) => {
  const index = errors.value.findIndex((e) => e.id === errorId);
  if (index !== -1) {
    errors.value.splice(index, 1);
  }
};

const formatFileSize = (bytes: number): string => {
  if (bytes === 0) return "0 Bytes";

  const k = 1024;
  const sizes = ["Bytes", "KB", "MB", "GB", "TB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
};

// Public methods for parent components
defineExpose({
  uploadFiles,
  clearAllFiles,
  files: files.value,
});
</script>

<style scoped>
.drag-over {
  border-color: #3b82f6;
  background-color: #eff6ff;
}
</style>
