export interface User {
  userId?: number
  username: string
  email: string
  role: string
  isActive?: boolean
  createdAt?: string
}

export interface AuthResponse {
  token: string
  username: string
  role: string
  expiresAt: string
}

export interface Patient {
  patientId?: number
  name: string
  age: number
  insurance: string
  address?: string
  phone?: string
  email?: string
  dateOfBirth?: string
  doctorId?: number
}

export interface Doctor {
  doctorId?: number
  name: string
  specialization: string
  consultationFee: number
  email?: string
  phone?: string
  departmentId: number
}

export interface Appointment {
  appointmentId?: number
  patientId: number
  doctorId: number
  time: string
  status: string
  reason: string
  createdAt?: string
  patientName?: string
  doctorName?: string
  doctorSpecialization?: string
  patient?: Patient
  doctor?: Doctor
}

export interface Department {
  departmentId?: number
  name: string
}

export interface Invoice {
  invoiceId?: number
  invoiceNumber?: string
  patientId: number
  total: number
  status: string
  dateIssued?: string
  datePaid?: string
  patientName?: string
  patient?: Patient
}

export interface LabResult {
  resultId?: number
  patientId: number
  doctorId?: number
  nurseId?: number
  type: string
  resultData?: string
  testDate?: string
  resultDate?: string
}

export interface Prescription {
  prescriptionId?: number
  doctorId: number
  patientId: number
  instructions: string
  medication?: string
  dosage?: string
  issuedDate?: string
  expiryDate?: string
}

export interface ClinicalEntry {
  entryId: number
  date: string
  notes: string
  diagnosis?: string
}

export interface MedicalRecord {
  recordId: number
  patientId: number
  patientName: string
  diagnosis?: string
  treatment?: string
  createdAt: string
  clinicalEntries: ClinicalEntry[]
}

