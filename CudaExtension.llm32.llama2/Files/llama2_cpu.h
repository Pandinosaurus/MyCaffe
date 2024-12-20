#ifndef __LLAMA2_CPU_H_
#define __LLAMA2_CPU_H_

#include <sys/types.h>
#include <sys/stat.h>
#include <io.h>
#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <time.h>
#include <math.h>
#include <string.h>
#include <fcntl.h>
#if defined _WIN32
#include "win.h"
#else
#include <unistd.h>
#include <sys/mman.h>
#endif

#include "config.h"
#include "primitives.h"

//----------------------------------------------------------------------------
// Transformer model on CPU
//----------------------------------------------------------------------------

typedef struct {
    // token embedding table
    float* token_embedding_table;    // (vocab_size, dim)
    // weights for rmsnorms
    float* rms_att_weight; // (layer, dim) rmsnorm weights
    float* rms_ffn_weight; // (layer, dim)
    // weights for matmuls. note dim == n_heads * head_size
    float* wq; // (layer, dim, n_heads * head_size)
    float* wk; // (layer, dim, n_kv_heads * head_size)
    float* wv; // (layer, dim, n_kv_heads * head_size)
    float* wo; // (layer, n_heads * head_size, dim)
    // weights for ffn
    float* w1; // (layer, hidden_dim, dim)
    float* w2; // (layer, dim, hidden_dim)
    float* w3; // (layer, hidden_dim, dim)
    // final rmsnorm
    float* rms_final_weight; // (dim,)
    // (optional) classifier weights for the logits, on the last layer
    float* wcls;
} TransformerWeights;

typedef struct {
    // current wave of activations
    float* x; // activation at current time stamp (dim,)
    float* xb; // same, but inside a residual branch (dim,)
    float* xb2; // an additional buffer just for convenience (dim,)
    float* hb; // buffer for hidden dimension in the ffn (hidden_dim,)
    float* hb2; // buffer for hidden dimension in the ffn (hidden_dim,)
    float* q; // query (dim,)
    float* k; // key (dim,)
    float* v; // value (dim,)
    float* att; // buffer for scores/attention values (n_heads, seq_len)
    float* logits; // output logits
    // kv cache
    float* key_cache;   // (layer, seq_len, dim)
    float* value_cache; // (layer, seq_len, dim)
    float* freq_real; // (seq_len, n_heads, head_size/2)
    float* freq_imag; // (seq_len, n_heads, head_size/2)
} RunState;

class TransformerCpu : public Transformer
{
public:
    TransformerWeights m_weights; // the weights of the model
    RunState m_state; // buffers for the "wave" of activations in the forward pass
    // some more state needed to properly clean up the memory mapping (sigh)
    int fd; // file descriptor for memory mapping
    float* data; // memory mapped data pointer
    ssize_t file_size; // size of the checkpoint file in bytes

    TransformerCpu() : m_state(), m_weights()
    {
        fd = -1;
        data = NULL;
        file_size = 0;
    }

    ~TransformerCpu()
    {
        cleanup();
    }

    long build(const char* checkpoint_path);
    float* forward(int token, int pos);
    void cleanup();
};

void softmax(float* x, int size);
void precompute_freqs_cis(int dim, size_t seq_len, float* freq_real, float* freq_imag);

#endif // __LLAMA2_CPU_H_
