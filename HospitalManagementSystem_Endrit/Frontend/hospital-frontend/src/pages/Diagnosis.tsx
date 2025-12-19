import { useEffect, useState } from 'react'
import { medicalRecordsApi, patientsApi } from '../services/api'
import { useAuth } from '../context/AuthContext'
import type { MedicalRecord, Patient } from '../types'
import './Common.css'

export default function Diagnosis() {
  const { user } = useAuth()
  const [medicalRecord, setMedicalRecord] = useState<MedicalRecord | null>(null)
  const [patient, setPatient] = useState<Patient | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      setLoading(true)
      // Find patient by email (assuming user email matches patient email)
      const patients = await patientsApi.getAll()
      const foundPatient = patients.find(p => p.email === user?.email || p.name === user?.username)
      
      if (!foundPatient) {
        setError('Nuk u gjet rekord mjekësor për këtë pacient')
        setLoading(false)
        return
      }

      setPatient(foundPatient)
      
      try {
        const record = await medicalRecordsApi.getByPatient(foundPatient.patientId!)
        setMedicalRecord(record)
      } catch (err) {
        // Medical record might not exist yet
        setError('Nuk u gjet rekord mjekësor për këtë pacient')
      }
    } catch (err: any) {
      console.error('Error loading data:', err)
      setError('Gabim në ngarkimin e të dhënave')
    } finally {
      setLoading(false)
    }
  }

  if (loading) return <div>Loading...</div>

  if (error && !medicalRecord) {
    return (
      <div className="page-container">
        <h1>Diagnoza</h1>
        <div className="error-message">{error}</div>
      </div>
    )
  }

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Diagnoza dhe Rekordet Mjekësore</h1>
      </div>

      {medicalRecord && (
        <div className="medical-record">
          <div className="record-section">
            <h2>Informacione Bazë</h2>
            <p><strong>Pacienti:</strong> {medicalRecord.patientName}</p>
            <p><strong>Data e Krijimit:</strong> {new Date(medicalRecord.createdAt).toLocaleDateString()}</p>
          </div>

          {medicalRecord.diagnosis && (
            <div className="record-section">
              <h2>Diagnoza</h2>
              <p>{medicalRecord.diagnosis}</p>
            </div>
          )}

          {medicalRecord.treatment && (
            <div className="record-section">
              <h2>Trajtimi</h2>
              <p>{medicalRecord.treatment}</p>
            </div>
          )}

          {medicalRecord.clinicalEntries && medicalRecord.clinicalEntries.length > 0 && (
            <div className="record-section">
              <h2>Hyrjet Klinike</h2>
              <table className="data-table">
                <thead>
                  <tr>
                    <th>Data</th>
                    <th>Shënime</th>
                    <th>Diagnoza</th>
                  </tr>
                </thead>
                <tbody>
                  {medicalRecord.clinicalEntries.map((entry) => (
                    <tr key={entry.entryId}>
                      <td>{new Date(entry.date).toLocaleDateString()}</td>
                      <td>{entry.notes}</td>
                      <td>{entry.diagnosis || '-'}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {!medicalRecord.diagnosis && !medicalRecord.treatment && 
           (!medicalRecord.clinicalEntries || medicalRecord.clinicalEntries.length === 0) && (
            <div className="record-section">
              <p>Nuk ka informacione të disponueshme për këtë rekord mjekësor.</p>
            </div>
          )}
        </div>
      )}
    </div>
  )
}

