import { useEffect, useState } from 'react'
import { medicalRecordsApi, patientsApi } from '../services/api'
import type { MedicalRecord, Patient } from '../types'
import './Common.css'

export default function MedicalRecords() {
  const [patients, setPatients] = useState<Patient[]>([])
  const [selectedPatientId, setSelectedPatientId] = useState<number>(0)
  const [medicalRecord, setMedicalRecord] = useState<MedicalRecord | null>(null)
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [showEntryModal, setShowEntryModal] = useState(false)
  const [formData, setFormData] = useState({
    diagnosis: '',
    treatment: '',
  })
  const [entryData, setEntryData] = useState({
    notes: '',
    diagnosis: '',
  })

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const patientsData = await patientsApi.getAll()
      setPatients(patientsData)
    } catch (error) {
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const loadMedicalRecord = async (patientId: number) => {
    try {
      const record = await medicalRecordsApi.getByPatient(patientId)
      setMedicalRecord(record)
      setSelectedPatientId(patientId)
    } catch (error: any) {
      if (error.message?.includes('404') || error.message?.includes('Not Found')) {
        setMedicalRecord(null)
        setSelectedPatientId(patientId)
      } else {
        console.error('Error loading medical record:', error)
        alert('Gabim në ngarkimin e rekordit mjekësor')
      }
    }
  }

  const handleCreateRecord = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!selectedPatientId) {
      alert('Ju lutem zgjidhni një pacient')
      return
    }

    try {
      await medicalRecordsApi.create({
        patientId: selectedPatientId,
        diagnosis: formData.diagnosis || undefined,
        treatment: formData.treatment || undefined,
      })
      setShowModal(false)
      setFormData({ diagnosis: '', treatment: '' })
      await loadMedicalRecord(selectedPatientId)
    } catch (error: any) {
      console.error('Error creating medical record:', error)
      alert(error?.message || 'Gabim në krijimin e rekordit mjekësor')
    }
  }

  const handleUpdateRecord = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!medicalRecord) return

    try {
      await medicalRecordsApi.update(medicalRecord.recordId, {
        diagnosis: formData.diagnosis || undefined,
        treatment: formData.treatment || undefined,
      })
      setShowModal(false)
      setFormData({ diagnosis: '', treatment: '' })
      await loadMedicalRecord(selectedPatientId)
    } catch (error: any) {
      console.error('Error updating medical record:', error)
      alert(error?.message || 'Gabim në përditësimin e rekordit mjekësor')
    }
  }

  const handleAddEntry = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!medicalRecord) return

    try {
      await medicalRecordsApi.addClinicalEntry({
        recordId: medicalRecord.recordId,
        notes: entryData.notes,
        diagnosis: entryData.diagnosis || undefined,
      })
      setShowEntryModal(false)
      setEntryData({ notes: '', diagnosis: '' })
      await loadMedicalRecord(selectedPatientId)
    } catch (error: any) {
      console.error('Error adding clinical entry:', error)
      alert(error?.message || 'Gabim në shtimin e hyrjes klinike')
    }
  }

  const openEditModal = () => {
    if (medicalRecord) {
      setFormData({
        diagnosis: medicalRecord.diagnosis || '',
        treatment: medicalRecord.treatment || '',
      })
    }
    setShowModal(true)
  }

  if (loading) return <div>Loading...</div>

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Rekordet Mjekësore</h1>
        {medicalRecord && (
          <>
            <button onClick={openEditModal} className="btn-primary">
              Ndrysho Diagnozë/Trajtim
            </button>
            <button onClick={() => setShowEntryModal(true)} className="btn-primary" style={{ marginLeft: '10px' }}>
              + Shto Hyrje Klinike
            </button>
          </>
        )}
      </div>

      <div className="form-group" style={{ marginBottom: '20px' }}>
        <label>Zgjidhni Pacientin *</label>
        <select
          value={selectedPatientId}
          onChange={(e) => {
            const patientId = parseInt(e.target.value)
            if (patientId > 0) {
              loadMedicalRecord(patientId)
            } else {
              setSelectedPatientId(0)
              setMedicalRecord(null)
            }
          }}
          required
        >
          <option value={0}>Zgjidhni Pacientin</option>
          {patients.map((patient) => (
            <option key={patient.patientId} value={patient.patientId}>
              {patient.name}
            </option>
          ))}
        </select>
      </div>

      {!selectedPatientId && (
        <div style={{ textAlign: 'center', padding: '40px', color: '#7f8c8d' }}>
          Ju lutem zgjidhni një pacient për të shfaqur rekordet mjekësore
        </div>
      )}

      {selectedPatientId && !medicalRecord && (
        <div style={{ textAlign: 'center', padding: '40px' }}>
          <p style={{ marginBottom: '20px', color: '#7f8c8d' }}>
            Nuk ekziston rekord mjekësor për këtë pacient
          </p>
          <button
            onClick={() => {
              setFormData({ diagnosis: '', treatment: '' })
              setShowModal(true)
            }}
            className="btn-primary"
          >
            Krijo Rekord Mjekësor
          </button>
        </div>
      )}

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
        </div>
      )}

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>{medicalRecord ? 'Ndrysho Diagnozë/Trajtim' : 'Krijo Rekord Mjekësor'}</h2>
            <form onSubmit={medicalRecord ? handleUpdateRecord : handleCreateRecord}>
              <div className="form-group">
                <label>Diagnoza</label>
                <textarea
                  value={formData.diagnosis}
                  onChange={(e) => setFormData({ ...formData, diagnosis: e.target.value })}
                  rows={4}
                  style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }}
                  placeholder="Shkruani diagnozën..."
                />
              </div>
              <div className="form-group">
                <label>Trajtimi</label>
                <textarea
                  value={formData.treatment}
                  onChange={(e) => setFormData({ ...formData, treatment: e.target.value })}
                  rows={4}
                  style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }}
                  placeholder="Shkruani trajtimin..."
                />
              </div>
              <div className="form-actions">
                <button type="button" onClick={() => setShowModal(false)} className="btn-secondary">
                  Anulo
                </button>
                <button type="submit" className="btn-primary">
                  {medicalRecord ? 'Përditëso' : 'Krijo'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {showEntryModal && medicalRecord && (
        <div className="modal-overlay" onClick={() => setShowEntryModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>Shto Hyrje Klinike</h2>
            <form onSubmit={handleAddEntry}>
              <div className="form-group">
                <label>Shënime *</label>
                <textarea
                  value={entryData.notes}
                  onChange={(e) => setEntryData({ ...entryData, notes: e.target.value })}
                  rows={4}
                  required
                  style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }}
                  placeholder="Shkruani shënimet..."
                />
              </div>
              <div className="form-group">
                <label>Diagnoza</label>
                <input
                  type="text"
                  value={entryData.diagnosis}
                  onChange={(e) => setEntryData({ ...entryData, diagnosis: e.target.value })}
                  style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }}
                  placeholder="Shkruani diagnozën..."
                />
              </div>
              <div className="form-actions">
                <button type="button" onClick={() => setShowEntryModal(false)} className="btn-secondary">
                  Anulo
                </button>
                <button type="submit" className="btn-primary">
                  Shto
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

