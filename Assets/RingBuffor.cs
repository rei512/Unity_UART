// RingBuffer.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// �ő�e�ʕt�������O�o�b�t�@�[��\���܂��B
/// </summary>
public class RingBuffer<T> : IEnumerable<T>
{
    // -x-x- Fields -x-x-

    private readonly Queue<T> _queue; // �f�[�^�����ۂɊi�[����I�u�W�F�N�g

    // -x-x- Properties -x-x-

    /// <summary>
    /// �o�b�t�@�[�Ɋi�[����Ă���v�f�̐����擾���܂��B
    /// </summary>
    public int Count => this._queue.Count;

    /// <summary>
    /// ���̃o�b�t�@�[�̍ő�e�ʂ��擾���܂��B
    /// </summary>
    public int MaxCapacity { get; private set; }

    // -x-x- operators -x-x-

    /// <summary>
    /// �w�肵���ʒu�̗v�f���Q�Ƃ��܂��B
    /// </summary>
    public T this[int index]
    {
        get
        {
            if (index < 0 || index > this.Count)
                throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)}={index}");
            return _queue.ElementAt(index);
        }
    }

    // -x-x- Constructors -x-x-

    /// <summary>
    /// �����O�o�b�t�@�[�̍ő�v�f�������Ă����ăI�u�W�F�N�g�����������܂��B
    /// </summary>
    public RingBuffer(int maxCapacity)
    {
        this.MaxCapacity = maxCapacity;
        _queue = new Queue<T>(maxCapacity);
    }

    // -x-x- Public Methods -x-x-

    /// <summary>
    /// �w�肵���v�f�������O�o�b�t�@�[�ɒǉ����܂��B
    /// </summary>
    public void Add(T item)
    {
        _queue.Enqueue(item);
        if (_queue.Count > this.MaxCapacity)
        {
            T removed = this.Pop();

            // �f�o�b�O�p�̏o��:
            // Console.WriteLine($"�L���p�V�e�B�𒴂��Ă��邽�߃o�b�t�@�̐擪�f�[�^�j�����܂����B{removed}");
        }
    }

    /// <summary>
    /// �����̗v�f���擾���o�b�t�@�[����f�[�^���폜���܂��B
    /// </summary>
    public T Pop() => _queue.Dequeue();

    /// <summary>
    /// �o�b�t�@�[�̐擪�̗v�f���擾���܂��B�f�[�^�͍폜����܂���B
    /// </summary>
    public T First() => _queue.Peek();

    /// <summary>
    /// �w�肵���v�f�����݂��邩�ǂ������m�F���܂��B
    /// </summary>
    public bool Contains(T item) => _queue.Contains(item);

    /// <summary>
    /// ���̃I�u�W�F�N�g���Ǘ����̑S�f�[�^��z��Ƃ��Ď擾���܂��B
    /// </summary>
    public T[] ToArray() => _queue.ToArray();

    /// <summary>
    /// ���݂̃����O�o�b�t�@�[���̗v�f��񋓂��܂��B
    /// �iLinq�ɂ���荂�x�ȑ��������悤�ɂ��̃��\�b�h���`���Ă����j
    /// </summary>
    public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

    // IEnumerator �̖����I�Ȏ���
    IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
}